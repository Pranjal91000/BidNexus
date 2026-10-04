import { useEffect, useMemo, useRef, useState } from 'react'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import './App.css'

type Auction = {
  id: number
  docNoYearly: string
  docDate: string
  auctionIntentSubmissionDate?: string
  organization?: {
    id?: number
    name?: string
    about?: string
    foregroundImageId?: number | null
    officialAddress?: string | null
  }
  organizationId?: number
  auctionRequirements?: Requirement[]
  isForwardAuction?: boolean
  auctionStartTime?: string
  auctionEndTime?: string
  statusName?: string
  about?: string
}

type Requirement = {
  id: number
  lineNo: number
  itemId?: number
  item?: {
    id?: number
    itemName?: string
    name?: string
    code?: string
    docAttachmentId?: number | null
  }
  unit?: { alias?: string; name?: string }
  quantity: number
  technicalSpecification?: string
  documentAttachmentId?: number
}

type Bid = {
  id: number
  vendorId: number
  netAmount: number
  basicAmount: number
  taxAmount: number
  createdAt: string
  isCurrent: boolean
  bidRevisionNo: number
  vendor?: { name?: string }
}

type Statement = {
  id: number
  auctionId: number
  bidId: number
  vendorId: number
  vendorName: string
  netAmount: number
  rank: number
  isWinner: boolean
}

type TenantProfile = {
  tenantId: number
  name: string
  about: string
  foregroundImageId?: number | null
  foregroundImageUrl?: string | null
  role: string
  emailAddress: string
  userName: string
  contactNumber: string
  officialAddress?: string | null
  referenceId: number
}

type OrganizationPublicProfile = {
  id: number
  tenantId: number
  name: string
  about: string
  foregroundImageId?: number | null
  foregroundImageUrl?: string | null
  officialAddress?: string | null
  totalAuctionsCount: number
  liveAuctionsCount: number
}

type ItemMaster = {
  id: number
  name: string
  code: string
  categoryId: number
  itemDescription?: string
  docAttachmentId?: number | null
  docAttachmentUrl?: string | null
  statusId: number
  statusRemarks?: string
  unitIds: number[]
  createdDateTime?: string
  lastModifiedDateTime?: string
}

type GlobalOption = {
  id: number
  name: string
}

type UnitMaster = {
  id: number
  name: string
  code: string
}

const API = import.meta.env.VITE_API_URL || (import.meta.env.DEV ? 'http://localhost:5000' : '')
const money = new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 2 })

async function api<T>(path: string, token: string, options: RequestInit = {}) {
  const isFormData = options.body instanceof FormData
  const headers: Record<string, string> = {
    Authorization: `Bearer ${token}`,
    ...((options.headers as Record<string, string>) || {}),
  }
  if (!isFormData && !headers['Content-Type']) {
    headers['Content-Type'] = 'application/json'
  }
  const response = await fetch(`${API}${path}`, {
    ...options,
    headers,
  })
  if (!response.ok) {
    let errText = await response.text()
    try {
      const parsed = JSON.parse(errText)
      if (parsed.message) errText = parsed.message
    } catch {}
    throw new Error(errText || `Request failed: ${response.status}`)
  }
  return response.status === 204 ? (undefined as T) : (response.json() as Promise<T>)
}

function statusTone(status = '') {
  const s = status.toLowerCase()
  return s.includes('active') || s === 'open' ? 'live' : s.includes('close') || s.includes('complet') ? 'closed' : 'scheduled'
}

function App() {
  const [token, setToken] = useState(() => localStorage.getItem('bidnexus_token') || '')
  const [currentTab, setCurrentTab] = useState<'auctions' | 'items' | 'profile'>('auctions')
  const [selectedOrgId, setSelectedOrgId] = useState<number | null>(null)
  const [auctions, setAuctions] = useState<Auction[]>([])
  const [selected, setSelected] = useState<Auction | null>(null)
  const [bids, setBids] = useState<Bid[]>([])
  const [statement, setStatement] = useState<Statement[]>([])
  const [bidAmount, setBidAmount] = useState('')
  const [loading, setLoading] = useState(false)
  const [notice, setNotice] = useState('')
  const [search, setSearch] = useState('')
  const [realtime, setRealtime] = useState(false)
  const [profile, setProfile] = useState<TenantProfile | null>(null)

  useEffect(() => {
    if (!token) return
    localStorage.setItem('bidnexus_token', token)
    loadAuctions()
    loadProfile()
  }, [token])

  async function loadProfile() {
    if (!token) return
    try {
      const p = await api<TenantProfile>('/api/profile', token)
      setProfile(p)
    } catch {
      // Profile load is non-fatal if token doesn't have valid claims yet
    }
  }

  async function loadAuctions() {
    setLoading(true)
    try {
      setAuctions(await api<Auction[]>('/api/auctions?pageNo=1&pageSize=50', token))
    } catch (e) {
      setNotice(e instanceof Error ? e.message : 'Unable to load auctions')
    } finally {
      setLoading(false)
    }
  }

  async function openAuction(auction: Auction) {
    setSelected(auction)
    setSelectedOrgId(null)
    setStatement([])
    setNotice('')
    try {
      const [detail, leaderboard] = await Promise.all([
        api<Auction>(`/api/auctions/${auction.id}`, token),
        api<Bid[]>(`/api/bids/auction/${auction.id}/leaderboard`, token),
      ])
      setSelected(detail)
      setBids(leaderboard)
    } catch (e) {
      setNotice(e instanceof Error ? e.message : 'Unable to load auction')
    }
  }

  function viewOrganization(orgId: number) {
    setSelectedOrgId(orgId)
    setSelected(null)
    setCurrentTab('auctions')
  }

  async function submitBid() {
    if (!selected || !bidAmount) return
    const amount = Number(bidAmount)
    if (!Number.isFinite(amount) || amount <= 0) return setNotice('Enter a valid bid amount.')
    const details = (selected.auctionRequirements || []).map((r) => ({
      auctionRequirementId: r.id,
      rate: amount / Math.max(r.quantity, 1),
      baseAmount: amount,
      netAmount: amount,
      taxes: [],
    }))
    try {
      const result = await api<Bid>('/api/bids', token, {
        method: 'POST',
        body: JSON.stringify({
          auctionId: selected.id,
          vendorId: 0,
          basicAmount: amount,
          taxAmount: 0,
          discountAmount: 0,
          netAmount: amount,
          mainBidId: null,
          bidRevisionNo: 0,
          bidDetails: details,
        }),
      })
      setBids((current) => [{ ...result, isCurrent: true }, ...current])
      setBidAmount('')
      setNotice('Bid accepted. Other participants have been notified live.')
    } catch (e) {
      setNotice(e instanceof Error ? e.message : 'Bid could not be submitted')
    }
  }

  async function loadStatement() {
    if (!selected) return
    try {
      setStatement(await api<Statement[]>(`/api/auctions/${selected.id}/statement`, token))
    } catch (e) {
      setNotice(e instanceof Error ? e.message : 'Statement unavailable')
    }
  }

  useEffect(() => {
    if (!selected || !token) return

    const connection = new HubConnectionBuilder()
      .withUrl(`${API}/hubs/auction`, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('BidAccepted', (event: { AuctionId: number; BidId: number; VendorId: number; NetAmount: number }) => {
      const liveBid: Bid = {
        id: event.BidId,
        vendorId: event.VendorId,
        netAmount: event.NetAmount,
        basicAmount: event.NetAmount,
        taxAmount: 0,
        createdAt: new Date().toISOString(),
        isCurrent: true,
        bidRevisionNo: 0,
      }
      setBids((current) => [liveBid, ...current.filter((b) => b.id !== event.BidId)])
      setNotice('A new bid was accepted.')
    })

    connection.on('AuctionClosed', () => {
      setSelected((current) => (current ? { ...current, statusName: 'Closed' } : current))
      setNotice('Auction closed. Final statement is now available to the organization.')
    })

    connection
      .start()
      .then(() => connection.invoke('JoinAuction', selected.id))
      .then(() => setRealtime(true))
      .catch(() => setRealtime(false))

    return () => {
      setRealtime(false)
      connection.invoke('LeaveAuction', selected.id).catch(() => undefined)
      connection.stop()
    }
  }, [selected?.id, token])

  const filtered = useMemo(
    () =>
      auctions.filter((a) =>
        `${a.docNoYearly} ${a.organization?.name || ''} ${a.about || ''}`.toLowerCase().includes(search.toLowerCase())
      ),
    [auctions, search]
  )

  if (!token)
    return (
      <main className="auth-shell">
        <div className="auth-card">
          <div className="brand">
            <span className="brand-mark">B</span>
            <span>
              Bid<span>Nexus</span>
            </span>
          </div>
          <p className="eyebrow">AUCTION OPERATIONS</p>
          <h1>Enter the command center.</h1>
          <p className="muted">Paste a JWT from the BidNexus API to connect this frontend to your live environment.</p>
          <label>API token</label>
          <textarea value={token} onChange={(e) => setToken(e.target.value.trim())} placeholder="eyJhbGciOi..." />
          <button className="primary" disabled={!token.trim()} onClick={() => setToken(token.trim())}>
            Connect to BidNexus
          </button>
        </div>
      </main>
    )

  return (
    <div className="app-shell">
      <aside>
        <div className="brand">
          <span className="brand-mark">B</span>
          <span>
            Bid<span>Nexus</span>
          </span>
        </div>
        <nav>
          <button
            className={currentTab === 'auctions' && !selectedOrgId && !selected ? 'nav-active' : ''}
            onClick={() => {
              setCurrentTab('auctions')
              setSelected(null)
              setSelectedOrgId(null)
              loadAuctions()
            }}
          >
            Auctions
          </button>
          <button
            className={currentTab === 'items' ? 'nav-active' : ''}
            onClick={() => {
              setCurrentTab('items')
              setSelected(null)
              setSelectedOrgId(null)
            }}
          >
            Item Master
          </button>
          <button
            className={currentTab === 'profile' ? 'nav-active' : ''}
            onClick={() => {
              setCurrentTab('profile')
              setSelected(null)
              setSelectedOrgId(null)
            }}
          >
            Personal Info & Profile
          </button>
          <button
            onClick={() => {
              if (selected) loadStatement()
              else setNotice('Select an auction to view its final statement.')
            }}
          >
            Statements
          </button>
        </nav>
        <div className="side-bottom">
          {profile && (
            <div style={{ padding: '0 14px 12px', fontSize: 13, borderBottom: '1px solid #1e293b', marginBottom: 12 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                {profile.foregroundImageId ? (
                  <img
                    src={`${API}/api/attachments/${profile.foregroundImageId}`}
                    alt={profile.name}
                    style={{ width: 34, height: 34, borderRadius: 8, objectFit: 'cover' }}
                  />
                ) : (
                  <div
                    style={{
                      width: 34,
                      height: 34,
                      borderRadius: 8,
                      background: '#2563eb',
                      display: 'grid',
                      placeItems: 'center',
                      fontWeight: 800,
                      color: '#fff',
                    }}
                  >
                    {profile.name.charAt(0).toUpperCase() || 'U'}
                  </div>
                )}
                <div style={{ overflow: 'hidden' }}>
                  <div style={{ fontWeight: 700, whiteSpace: 'nowrap', textOverflow: 'ellipsis', overflow: 'hidden' }}>
                    {profile.name}
                  </div>
                  <div style={{ fontSize: 11, color: '#8fb8ff' }}>{profile.role}</div>
                </div>
              </div>
            </div>
          )}
          <div className="connection">
            <i /> API connected
          </div>
          <button
            className="logout"
            onClick={() => {
              localStorage.removeItem('bidnexus_token')
              setToken('')
            }}
          >
            Disconnect
          </button>
        </div>
      </aside>

      <main className="workspace">
        <header>
          <div>
            <p className="eyebrow">
              {currentTab === 'profile'
                ? 'PROFILE SETTINGS'
                : currentTab === 'items'
                ? 'MASTER CATALOG'
                : selectedOrgId
                ? 'ORGANIZATION PROFILE'
                : selected
                ? 'AUCTION DETAILS'
                : 'PROCUREMENT CONTROL'}
            </p>
            <h1>
              {currentTab === 'profile'
                ? 'Personal & Profile Information'
                : currentTab === 'items'
                ? 'Item Master'
                : selectedOrgId
                ? 'Organization Overview'
                : selected
                ? selected.docNoYearly || 'Auction Desk'
                : 'Live Auction Desk'}
            </h1>
          </div>
          <div className="header-actions">
            <span className="live-dot" />
            {realtime ? 'Realtime connected' : 'Realtime ready'}{' '}
            <button
              onClick={() => {
                loadAuctions()
                loadProfile()
              }}
              title="Refresh"
            >
              ↻
            </button>
          </div>
        </header>

        {notice && (
          <div className="notice">
            {notice}
            <button onClick={() => setNotice('')}>×</button>
          </div>
        )}

        {currentTab === 'profile' ? (
          <ProfileView
            token={token}
            profile={profile}
            onProfileUpdated={(updated) => {
              setProfile(updated)
              loadAuctions()
            }}
          />
        ) : currentTab === 'items' ? (
          <ItemMasterView token={token} />
        ) : selectedOrgId ? (
          <OrganizationProfileView
            orgId={selectedOrgId}
            token={token}
            onBack={() => setSelectedOrgId(null)}
            onOpenAuction={(a) => {
              setSelectedOrgId(null)
              openAuction(a)
            }}
          />
        ) : selected ? (
          <AuctionDesk
            auction={selected}
            bids={bids}
            bidAmount={bidAmount}
            setBidAmount={setBidAmount}
            submitBid={submitBid}
            statement={statement}
            loadStatement={loadStatement}
            onViewOrg={(orgId) => viewOrganization(orgId)}
            onBack={() => setSelected(null)}
          />
        ) : (
          <>
            <section className="stats">
              <div>
                <span>Visible auctions</span>
                <strong>{auctions.length}</strong>
              </div>
              <div>
                <span>Live now</span>
                <strong>{auctions.filter((a) => statusTone(a.statusName) === 'live').length}</strong>
              </div>
              <div>
                <span>Scheduled</span>
                <strong>{auctions.filter((a) => statusTone(a.statusName) === 'scheduled').length}</strong>
              </div>
              <div>
                <span>Closed</span>
                <strong>{auctions.filter((a) => statusTone(a.statusName) === 'closed').length}</strong>
              </div>
            </section>
            <div className="section-head">
              <div>
                <p className="eyebrow">AUCTION REGISTER</p>
                <h2>Opportunities</h2>
              </div>
              <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Search auction, buyer, item..." />
            </div>
            <section className="auction-grid">
              {loading ? (
                <div className="empty">Loading auctions…</div>
              ) : (
                filtered.map((a) => {
                  const orgId = a.organization?.id || a.organizationId || 0
                  return (
                    <div className="auction-card" key={a.id}>
                      <div className="card-top">
                        <span className={`status ${statusTone(a.statusName)}`}>{a.statusName || 'Scheduled'}</span>
                        <span>#{a.id}</span>
                      </div>
                      <h3>{a.docNoYearly || `Auction ${a.id}`}</h3>
                      <p style={{ marginTop: 4 }}>
                        {orgId > 0 ? (
                          <button
                            type="button"
                            className="org-link"
                            onClick={(e) => {
                              e.stopPropagation()
                              viewOrganization(orgId)
                            }}
                          >
                            🏢 {a.organization?.name || 'Organization'} →
                          </button>
                        ) : (
                          <span>{a.organization?.name || 'Procurement Organization'}</span>
                        )}
                      </p>

                      {/* Item Preview Strip */}
                      {(a.auctionRequirements || []).length > 0 && (
                        <div className="items-preview-strip">
                          {(a.auctionRequirements || []).slice(0, 5).map((req, idx) => {
                            const docId = req.item?.docAttachmentId
                            const itemName = req.item?.itemName || req.item?.name || 'Item'
                            return docId ? (
                              <img
                                key={idx}
                                src={`${API}/api/attachments/${docId}`}
                                alt={itemName}
                                title={`${itemName} (${req.quantity} ${req.unit?.alias || ''})`}
                                className="item-thumb-mini"
                              />
                            ) : (
                              <div
                                key={idx}
                                className="item-thumb-mini-placeholder"
                                title={`${itemName} (${req.quantity} ${req.unit?.alias || ''})`}
                              >
                                {itemName.slice(0, 2).toUpperCase()}
                              </div>
                            )
                          })}
                          {(a.auctionRequirements || []).length > 5 && (
                            <span style={{ fontSize: 10, color: '#64748b' }}>
                              +{a.auctionRequirements!.length - 5}
                            </span>
                          )}
                        </div>
                      )}

                      <div className="card-foot">
                        <span>{a.isForwardAuction ? 'Forward' : 'Reverse'} auction</span>
                        <button
                          type="button"
                          className="btn-secondary"
                          style={{ padding: '4px 10px', fontSize: 12 }}
                          onClick={() => openAuction(a)}
                        >
                          View desk →
                        </button>
                      </div>
                    </div>
                  )
                })
              )}
              {!loading && filtered.length === 0 && <div className="empty">No auctions match your search.</div>}
            </section>
          </>
        )}
      </main>
    </div>
  )
}

function AuctionDesk({
  auction,
  bids,
  bidAmount,
  setBidAmount,
  submitBid,
  statement,
  loadStatement,
  onViewOrg,
  onBack,
}: {
  auction: Auction
  bids: Bid[]
  bidAmount: string
  setBidAmount: (v: string) => void
  submitBid: () => void
  statement: Statement[]
  loadStatement: () => void
  onViewOrg: (orgId: number) => void
  onBack: () => void
}) {
  const best = bids.length ? bids[0].netAmount : null
  const orgId = auction.organization?.id || auction.organizationId || 0

  return (
    <div className="desk">
      <button className="back" onClick={onBack}>
        ← Back to auctions
      </button>
      <div className="auction-hero">
        <div>
          <p className="eyebrow">
            {auction.isForwardAuction ? 'FORWARD AUCTION' : 'REVERSE AUCTION'} · #{auction.id}
          </p>
          <h1>{auction.docNoYearly || 'Auction'}</h1>
          <p className="muted" style={{ marginTop: 6, display: 'flex', alignItems: 'center', gap: 8 }}>
            {orgId > 0 ? (
              <button type="button" className="org-link" onClick={() => onViewOrg(orgId)}>
                🏢 {auction.organization?.name || 'Organization'}
              </button>
            ) : (
              <span>{auction.organization?.name || 'Organization'}</span>
            )}
            <span>·</span>
            <span>{auction.auctionRequirements?.length || 0} requirements</span>
          </p>
        </div>
        <span className={`status large ${statusTone(auction.statusName)}`}>{auction.statusName || 'Scheduled'}</span>
      </div>

      <div className="desk-grid">
        <section className="panel bid-panel">
          <div className="panel-head">
            <div>
              <p className="eyebrow">YOUR BID</p>
              <h2>{auction.isForwardAuction ? 'Submit your offer' : 'Submit your lowest offer'}</h2>
            </div>
            <span className="live-pill">
              <i /> LIVE
            </span>
          </div>
          <div className="amount-wrap">
            <span>₹</span>
            <input type="number" value={bidAmount} onChange={(e) => setBidAmount(e.target.value)} placeholder="0.00" />
            <small>Net amount</small>
          </div>
          {best !== null && (
            <div className="best-row">
              <span>Current market position</span>
              <strong>{money.format(best)}</strong>
            </div>
          )}
          <button className="primary wide" onClick={submitBid}>
            Submit bid <span>↗</span>
          </button>
          <p className="hint">
            Server calculates quantity, taxes and final net amount. Client values are treated as preview only.
          </p>
        </section>

        <section className="panel">
          <div className="panel-head">
            <div>
              <p className="eyebrow">LIVE MARKET</p>
              <h2>Leaderboard</h2>
            </div>
            <span className="live-pill">
              <i /> LIVE
            </span>
          </div>
          <div className="leaderboard">
            {bids.slice(0, 8).map((b, i) => (
              <div className={`leader ${i === 0 ? 'leader-first' : ''}`} key={b.id}>
                <b>{String(i + 1).padStart(2, '0')}</b>
                <span>{b.vendor?.name || `Vendor #${b.vendorId}`}</span>
                <strong>{money.format(b.netAmount)}</strong>
              </div>
            ))}
            {!bids.length && <div className="empty">No bids yet.</div>}
          </div>
        </section>
      </div>

      <section className="panel requirements">
        <div className="panel-head">
          <div>
            <p className="eyebrow">SCOPE & TECHNICAL SPECIFICATIONS</p>
            <h2>Item Requirements</h2>
          </div>
        </div>
        {(auction.auctionRequirements || []).map((r) => {
          const docId = r.item?.docAttachmentId
          const itemName = r.item?.itemName || r.item?.name || `Requirement #${r.id}`
          return (
            <div className="requirement" key={r.id}>
              {/* Item Square Thumbnail */}
              {docId ? (
                <img
                  src={`${API}/api/attachments/${docId}`}
                  alt={itemName}
                  className="item-thumb"
                  title="Item specification photo"
                />
              ) : (
                <div className="item-thumb-placeholder">{itemName.slice(0, 2).toUpperCase()}</div>
              )}
              <div>
                <strong>{itemName}</strong>
                <small>{r.technicalSpecification || 'No technical specification provided'}</small>
              </div>
              <b>
                {r.quantity} {r.unit?.alias || r.unit?.name || ''}
              </b>
            </div>
          )
        })}
      </section>

      <section className="panel statement">
        <div className="panel-head">
          <div>
            <p className="eyebrow">FINAL RESULT</p>
            <h2>Auction statement</h2>
          </div>
          <button onClick={loadStatement}>Load statement</button>
        </div>
        {statement.length ? (
          statement.map((s) => (
            <div className="winner" key={s.id}>
              <div className="winner-icon">✓</div>
              <div>
                <p className="eyebrow">WINNER</p>
                <h3>{s.vendorName}</h3>
                <span>
                  Bid #{s.bidId} · Rank {s.rank}
                </span>
              </div>
              <strong>{money.format(s.netAmount)}</strong>
            </div>
          ))
        ) : (
          <p className="muted statement-empty">
            Statement becomes available after the auction closes and is visible to the organization.
          </p>
        )}
      </section>
    </div>
  )
}

function OrganizationProfileView({
  orgId,
  token,
  onBack,
  onOpenAuction,
}: {
  orgId: number
  token: string
  onBack: () => void
  onOpenAuction: (a: Auction) => void
}) {
  const [org, setOrg] = useState<OrganizationPublicProfile | null>(null)
  const [auctions, setAuctions] = useState<Auction[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    async function loadData() {
      setLoading(true)
      try {
        const [orgData, orgAuctions] = await Promise.all([
          api<OrganizationPublicProfile>(`/api/organizations/${orgId}/public`, token),
          api<Auction[]>(`/api/organizations/${orgId}/auctions`, token),
        ])
        setOrg(orgData)
        setAuctions(orgAuctions)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Unable to load organization details.')
      } finally {
        setLoading(false)
      }
    }
    loadData()
  }, [orgId, token])

  if (loading) return <div className="empty">Loading organization profile…</div>
  if (error || !org)
    return (
      <div>
        <button className="back" onClick={onBack}>
          ← Back
        </button>
        <div className="empty">{error || 'Organization not found.'}</div>
      </div>
    )

  return (
    <div className="org-profile">
      <button className="back" onClick={onBack}>
        ← Back to auctions
      </button>

      {/* Hero Card */}
      <div className="org-hero-card">
        <div className="org-hero-banner">
          {org.foregroundImageId ? (
            <img src={`${API}/api/attachments/${org.foregroundImageId}`} alt={org.name} />
          ) : (
            <div
              style={{
                position: 'absolute',
                inset: 0,
                background: 'linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #3b82f6 100%)',
              }}
            />
          )}
          <div className="org-hero-avatar-wrap">
            {org.foregroundImageId ? (
              <img
                src={`${API}/api/attachments/${org.foregroundImageId}`}
                alt={org.name}
                className="org-hero-avatar"
              />
            ) : (
              <div className="org-hero-avatar">{org.name.charAt(0).toUpperCase()}</div>
            )}
          </div>
        </div>
        <div className="org-hero-body">
          <h2>{org.name}</h2>
          {org.officialAddress && <p className="muted" style={{ marginTop: 4 }}>📍 {org.officialAddress}</p>}
          <div className="org-meta-badges">
            <span className="org-meta-badge">Verified Organization</span>
            <span className="org-meta-badge">{org.totalAuctionsCount} total auctions conducted</span>
            <span className="org-meta-badge">{org.liveAuctionsCount} currently active</span>
          </div>
        </div>
      </div>

      {/* About Section */}
      <div className="about-card">
        <p className="eyebrow">ABOUT THE ORGANIZATION</p>
        <h3>Corporate Profile & Procurement Overview</h3>
        <p className="about-text">{org.about || 'No detailed background provided yet by this organization.'}</p>
      </div>

      {/* Conducted Auctions */}
      <section>
        <div className="section-head">
          <div>
            <p className="eyebrow">AUCTION HISTORY</p>
            <h2>Auctions Conducted by {org.name}</h2>
          </div>
          <span style={{ fontSize: 13, color: '#64748b' }}>{auctions.length} auctions found</span>
        </div>

        <div className="auction-grid">
          {auctions.map((a) => (
            <div className="auction-card" key={a.id}>
              <div className="card-top">
                <span className={`status ${statusTone(a.statusName)}`}>{a.statusName || 'Scheduled'}</span>
                <span>#{a.id}</span>
              </div>
              <h3>{a.docNoYearly || `Auction ${a.id}`}</h3>
              <p>{(a.auctionRequirements || []).length} requirement lines specified</p>
              <div className="card-foot">
                <span>{a.isForwardAuction ? 'Forward' : 'Reverse'}</span>
                <button
                  type="button"
                  className="btn-secondary"
                  style={{ padding: '4px 10px', fontSize: 12 }}
                  onClick={() => onOpenAuction(a)}
                >
                  View auction →
                </button>
              </div>
            </div>
          ))}
          {auctions.length === 0 && <div className="empty">No auctions conducted by this organization yet.</div>}
        </div>
      </section>
    </div>
  )
}

function ItemMasterView({ token }: { token: string }) {
  const [items, setItems] = useState<ItemMaster[]>([])
  const [categories, setCategories] = useState<GlobalOption[]>([])
  const [statuses, setStatuses] = useState<GlobalOption[]>([])
  const [units, setUnits] = useState<UnitMaster[]>([])
  const [loading, setLoading] = useState(false)
  const [search, setSearch] = useState('')
  const [notice, setNotice] = useState('')

  // Modal / Form state
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingItem, setEditingItem] = useState<ItemMaster | null>(null)
  const [formName, setFormName] = useState('')
  const [formCode, setFormCode] = useState('')
  const [formCategoryId, setFormCategoryId] = useState<number>(1)
  const [formStatusId, setFormStatusId] = useState<number>(1)
  const [formDescription, setFormDescription] = useState('')
  const [formUnitIds, setFormUnitIds] = useState<number[]>([])
  const [formDocAttachmentId, setFormDocAttachmentId] = useState<number | null>(null)
  const [formImagePreviewUrl, setFormImagePreviewUrl] = useState<string | null>(null)
  const [uploadingImage, setUploadingImage] = useState(false)
  const [saving, setSaving] = useState(false)
  const [formError, setFormError] = useState('')
  const fileInputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    loadAll()
  }, [token])

  async function loadAll() {
    setLoading(true)
    try {
      const [itemList, catList, statList, unitList] = await Promise.all([
        api<ItemMaster[]>('/api/masters/items', token),
        api<GlobalOption[]>('/api/global-data/categories', token).catch(() => []),
        api<GlobalOption[]>('/api/global-data/statuses', token).catch(() => []),
        api<UnitMaster[]>('/api/masters/units', token).catch(() => []),
      ])
      setItems(itemList)
      setCategories(catList)
      setStatuses(statList)
      setUnits(unitList)
    } catch (err) {
      setNotice(err instanceof Error ? err.message : 'Failed to load item master data.')
    } finally {
      setLoading(false)
    }
  }

  function openCreateModal() {
    setEditingItem(null)
    setFormName('')
    setFormCode('')
    setFormCategoryId(categories[0]?.id || 1)
    setFormStatusId(statuses[0]?.id || 1)
    setFormDescription('')
    setFormUnitIds([])
    setFormDocAttachmentId(null)
    setFormImagePreviewUrl(null)
    setFormError('')
    setIsModalOpen(true)
  }

  function openEditModal(item: ItemMaster) {
    setEditingItem(item)
    setFormName(item.name)
    setFormCode(item.code)
    setFormCategoryId(item.categoryId)
    setFormStatusId(item.statusId)
    setFormDescription(item.itemDescription || '')
    setFormUnitIds(item.unitIds || [])
    setFormDocAttachmentId(item.docAttachmentId ?? null)
    setFormImagePreviewUrl(item.docAttachmentId ? `${API}/api/attachments/${item.docAttachmentId}` : null)
    setFormError('')
    setIsModalOpen(true)
  }

  async function handleImageUpload(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0]
    if (!file) return

    setUploadingImage(true)
    setFormError('')
    try {
      const formData = new FormData()
      formData.append('file', file)

      const attachment = await api<{ id: number; url: string }>('/api/attachments/upload', token, {
        method: 'POST',
        body: formData,
      })

      setFormDocAttachmentId(attachment.id)
      setFormImagePreviewUrl(`${API}/api/attachments/${attachment.id}`)
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Failed to upload item image.')
    } finally {
      setUploadingImage(false)
    }
  }

  function handleRemoveImage() {
    setFormDocAttachmentId(null)
    setFormImagePreviewUrl(null)
    if (fileInputRef.current) fileInputRef.current.value = ''
  }

  function toggleUnit(unitId: number) {
    setFormUnitIds((curr) =>
      curr.includes(unitId) ? curr.filter((id) => id !== unitId) : [...curr, unitId]
    )
  }

  async function handleSave(e: React.FormEvent) {
    e.preventDefault()
    if (!formName.trim() || !formCode.trim()) {
      setFormError('Item Name and Item Code are required.')
      return
    }

    setSaving(true)
    setFormError('')
    try {
      const payload = {
        name: formName.trim(),
        code: formCode.trim().toUpperCase(),
        categoryId: Number(formCategoryId),
        itemDescription: formDescription.trim(),
        docAttachmentId: formDocAttachmentId,
        statusId: Number(formStatusId),
        statusRemarks: '',
        unitIds: formUnitIds,
      }

      if (editingItem) {
        await api<ItemMaster>(`/api/masters/items/${editingItem.id}`, token, {
          method: 'PUT',
          body: JSON.stringify(payload),
        })
        setNotice('Item updated successfully. Replaced or removed image has been purged from disk storage.')
      } else {
        await api<ItemMaster>('/api/masters/items', token, {
          method: 'POST',
          body: JSON.stringify(payload),
        })
        setNotice('New item created successfully with image attachment.')
      }

      setIsModalOpen(false)
      loadAll()
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Failed to save item.')
    } finally {
      setSaving(false)
    }
  }

  async function handleDelete(item: ItemMaster) {
    if (!window.confirm(`Are you sure you want to delete item "${item.name}"? Any attached image will be permanently purged from disk.`)) {
      return
    }

    try {
      await api(`/api/masters/items/${item.id}`, token, { method: 'DELETE' })
      setNotice(`Item "${item.name}" deleted and image storage reclaimed.`)
      loadAll()
    } catch (err) {
      setNotice(err instanceof Error ? err.message : 'Failed to delete item.')
    }
  }

  const filteredItems = items.filter(
    (item) =>
      item.name.toLowerCase().includes(search.toLowerCase()) ||
      item.code.toLowerCase().includes(search.toLowerCase()) ||
      (item.itemDescription || '').toLowerCase().includes(search.toLowerCase())
  )

  const getCategoryName = (catId: number) =>
    categories.find((c) => c.id === catId)?.name || `Category ${catId}`

  const getStatusName = (statId: number) =>
    statuses.find((s) => s.id === statId)?.name || (statId === 1 ? 'Active' : 'Inactive')

  return (
    <div>
      {notice && (
        <div className="notice" style={{ marginBottom: 20 }}>
          {notice}
          <button onClick={() => setNotice('')}>×</button>
        </div>
      )}

      <div className="section-head">
        <div>
          <p className="eyebrow">CATALOG REPOSITORY</p>
          <h2>Item Master Management</h2>
        </div>
        <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
          <input
            type="search"
            placeholder="Search items by code or name..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <button type="button" className="primary" onClick={openCreateModal}>
            + Create Item
          </button>
        </div>
      </div>

      <div className="item-table-card">
        <div className="table-responsive">
          <table className="item-table">
            <thead>
              <tr>
                <th style={{ width: 60 }}>Image</th>
                <th>Item Code</th>
                <th>Item Name</th>
                <th>Category</th>
                <th>Description</th>
                <th>Status</th>
                <th style={{ textAlign: 'right' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredItems.map((item) => (
                <tr key={item.id}>
                  <td>
                    {item.docAttachmentId ? (
                      <img
                        src={`${API}/api/attachments/${item.docAttachmentId}`}
                        alt={item.name}
                        className="item-thumb"
                      />
                    ) : (
                      <div className="item-thumb-placeholder">
                        {item.name ? item.name.substring(0, 2).toUpperCase() : 'IT'}
                      </div>
                    )}
                  </td>
                  <td>
                    <span className="item-code-badge">{item.code}</span>
                  </td>
                  <td>
                    <strong>{item.name}</strong>
                  </td>
                  <td>{getCategoryName(item.categoryId)}</td>
                  <td style={{ color: '#64748b', maxWidth: 220 }}>
                    {item.itemDescription ? (
                      item.itemDescription.length > 60
                        ? `${item.itemDescription.substring(0, 60)}…`
                        : item.itemDescription
                    ) : (
                      <span className="muted">—</span>
                    )}
                  </td>
                  <td>
                    <span className={item.statusId === 1 ? 'badge-active' : 'badge-inactive'}>
                      {getStatusName(item.statusId)}
                    </span>
                  </td>
                  <td style={{ textAlign: 'right', whiteSpace: 'nowrap' }}>
                    <button
                      type="button"
                      className="btn-secondary"
                      style={{ padding: '5px 10px', fontSize: 12, marginRight: 6 }}
                      onClick={() => openEditModal(item)}
                    >
                      Edit
                    </button>
                    <button
                      type="button"
                      className="btn-danger"
                      style={{ padding: '5px 10px', fontSize: 12 }}
                      onClick={() => handleDelete(item)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
              {filteredItems.length === 0 && !loading && (
                <tr>
                  <td colSpan={7} style={{ textAlign: 'center', padding: '40px 20px', color: '#94a3b8' }}>
                    No items found matching your search. Click "+ Create Item" to add one.
                  </td>
                </tr>
              )}
              {loading && (
                <tr>
                  <td colSpan={7} style={{ textAlign: 'center', padding: '40px 20px', color: '#94a3b8' }}>
                    Loading item catalog…
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Modal for Create / Edit Item */}
      {isModalOpen && (
        <div className="modal-backdrop" onClick={() => !saving && setIsModalOpen(false)}>
          <div className="modal-card" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <div>
                <p className="eyebrow">{editingItem ? 'UPDATE ITEM MASTER' : 'NEW MASTER RECORD'}</p>
                <h2>{editingItem ? `Edit ${editingItem.name}` : 'Create New Item'}</h2>
              </div>
              <button
                type="button"
                className="modal-close"
                disabled={saving}
                onClick={() => setIsModalOpen(false)}
              >
                ✕
              </button>
            </div>

            {formError && (
              <div className="error-callout">
                <span>⚠️</span>
                <div>{formError}</div>
              </div>
            )}

            <form onSubmit={handleSave}>
              {/* Square Image Attachment Section */}
              <div className="avatar-upload" style={{ alignItems: 'center' }}>
                {formImagePreviewUrl ? (
                  <img
                    src={formImagePreviewUrl}
                    alt="Item Preview"
                    style={{
                      width: 72,
                      height: 72,
                      borderRadius: 10,
                      objectFit: 'cover',
                      border: '1px solid #cbd5e1',
                    }}
                  />
                ) : (
                  <div
                    style={{
                      width: 72,
                      height: 72,
                      borderRadius: 10,
                      background: '#f1f5f9',
                      border: '1px dashed #94a3b8',
                      display: 'grid',
                      placeItems: 'center',
                      fontSize: 24,
                    }}
                  >
                    📦
                  </div>
                )}
                <div className="avatar-actions">
                  <strong style={{ fontSize: 13, color: '#1e293b' }}>Square Item Preview Image</strong>
                  <small className="muted">
                    Displayed gracefully in square aspect ratio on auction requirement cards. Replacing or removing an image automatically purges the old file from disk storage.
                  </small>
                  <div style={{ display: 'flex', gap: 8, marginTop: 4 }}>
                    <input
                      type="file"
                      ref={fileInputRef}
                      style={{ display: 'none' }}
                      accept="image/png,image/jpeg,image/webp,image/svg+xml"
                      onChange={handleImageUpload}
                    />
                    <button
                      type="button"
                      className="btn-secondary"
                      disabled={uploadingImage}
                      onClick={() => fileInputRef.current?.click()}
                    >
                      {uploadingImage ? 'Uploading…' : formImagePreviewUrl ? 'Change image' : 'Upload image'}
                    </button>
                    {formImagePreviewUrl && (
                      <button type="button" className="btn-danger" onClick={handleRemoveImage}>
                        Remove image
                      </button>
                    )}
                  </div>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label>Item Name *</label>
                  <input
                    type="text"
                    required
                    value={formName}
                    onChange={(e) => setFormName(e.target.value)}
                    placeholder="e.g. MS Plate 10mm"
                  />
                </div>
                <div className="form-group">
                  <label>Item Code *</label>
                  <input
                    type="text"
                    required
                    value={formCode}
                    onChange={(e) => setFormCode(e.target.value)}
                    placeholder="e.g. MSP-10"
                  />
                </div>
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label>Category *</label>
                  <select
                    value={formCategoryId}
                    onChange={(e) => setFormCategoryId(Number(e.target.value))}
                    style={{
                      padding: '12px 14px',
                      border: '1px solid #cbd5e1',
                      borderRadius: 8,
                      fontSize: 14,
                      background: '#fff',
                    }}
                  >
                    {categories.map((c) => (
                      <option key={c.id} value={c.id}>
                        {c.name}
                      </option>
                    ))}
                    {categories.length === 0 && <option value={1}>General Material</option>}
                  </select>
                </div>
                <div className="form-group">
                  <label>Status *</label>
                  <select
                    value={formStatusId}
                    onChange={(e) => setFormStatusId(Number(e.target.value))}
                    style={{
                      padding: '12px 14px',
                      border: '1px solid #cbd5e1',
                      borderRadius: 8,
                      fontSize: 14,
                      background: '#fff',
                    }}
                  >
                    {statuses.map((s) => (
                      <option key={s.id} value={s.id}>
                        {s.name}
                      </option>
                    ))}
                    {statuses.length === 0 && (
                      <>
                        <option value={1}>Active</option>
                        <option value={2}>Inactive</option>
                      </>
                    )}
                  </select>
                </div>
              </div>

              <div className="form-group">
                <label>Technical & Item Description</label>
                <textarea
                  rows={3}
                  value={formDescription}
                  onChange={(e) => setFormDescription(e.target.value)}
                  placeholder="Material specifications, dimensions, tolerances, standards..."
                />
              </div>

              {units.length > 0 && (
                <div className="form-group">
                  <label>Applicable Units</label>
                  <div className="units-selection-grid">
                    {units.map((u) => (
                      <button
                        type="button"
                        key={u.id}
                        className={`unit-chip ${formUnitIds.includes(u.id) ? 'selected' : ''}`}
                        onClick={() => toggleUnit(u.id)}
                      >
                        {u.name} ({u.code})
                      </button>
                    ))}
                  </div>
                  <small className="muted">Select one or more valid measurement units for this item.</small>
                </div>
              )}

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 20 }}>
                <button
                  type="button"
                  className="btn-secondary"
                  disabled={saving}
                  onClick={() => setIsModalOpen(false)}
                >
                  Cancel
                </button>
                <button type="submit" className="primary" disabled={saving || uploadingImage}>
                  {saving ? 'Saving item…' : editingItem ? 'Update Item & Free Old Storage' : 'Create Item'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

function ProfileView({
  token,
  profile,
  onProfileUpdated,
}: {
  token: string
  profile: TenantProfile | null
  onProfileUpdated: (updated: TenantProfile) => void
}) {
  const [name, setName] = useState(profile?.name || '')
  const [about, setAbout] = useState(profile?.about || '')
  const [foregroundImageId, setForegroundImageId] = useState<number | null>(profile?.foregroundImageId ?? null)
  const [imagePreviewUrl, setImagePreviewUrl] = useState<string | null>(
    profile?.foregroundImageId ? `${API}/api/attachments/${profile.foregroundImageId}` : null
  )
  const [uploadingImage, setUploadingImage] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const fileInputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    if (profile) {
      setName(profile.name)
      setAbout(profile.about || '')
      setForegroundImageId(profile.foregroundImageId ?? null)
      setImagePreviewUrl(profile.foregroundImageId ? `${API}/api/attachments/${profile.foregroundImageId}` : null)
    }
  }, [profile])

  async function handleImageFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0]
    if (!file) return

    setUploadingImage(true)
    setError('')
    try {
      const formData = new FormData()
      formData.append('file', file)

      const attachment = await api<{ id: number; url: string }>('/api/attachments/upload', token, {
        method: 'POST',
        body: formData,
      })

      setForegroundImageId(attachment.id)
      setImagePreviewUrl(`${API}/api/attachments/${attachment.id}`)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to upload image.')
    } finally {
      setUploadingImage(false)
    }
  }

  function handleRemoveImage() {
    setForegroundImageId(null)
    setImagePreviewUrl(null)
    if (fileInputRef.current) fileInputRef.current.value = ''
  }

  async function handleSave(e: React.FormEvent) {
    e.preventDefault()
    if (!name.trim()) {
      setError('Name is required.')
      return
    }

    setSaving(true)
    setError('')
    setSuccess('')

    try {
      const updated = await api<TenantProfile>('/api/profile', token, {
        method: 'PUT',
        body: JSON.stringify({
          name: name.trim(),
          about: about.trim(),
          foregroundImageId: foregroundImageId,
        }),
      })

      setSuccess('Profile updated successfully. Previous assets have been cleaned up.')
      onProfileUpdated(updated)
    } catch (err) {
      // Backend returns 409 Conflict if name matches an existing tenant
      setError(err instanceof Error ? err.message : 'Failed to update profile.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div style={{ maxWidth: 720 }}>
      <div className="profile-edit-card">
        <p className="eyebrow">MANAGE PERSONAL & ENTITY INFORMATION</p>
        <h2>Edit {profile?.role || 'User'} Profile</h2>
        <p className="muted" style={{ marginBottom: 24, marginTop: 4 }}>
          Update your organization's legal name, corporate background, and foreground branding. Name uniqueness across all
          tenants is automatically verified.
        </p>

        {error && (
          <div className="error-callout">
            <span>⚠️</span>
            <div>{error}</div>
          </div>
        )}

        {success && <div className="success-callout">✓ {success}</div>}

        <form onSubmit={handleSave}>
          {/* Foreground Image Upload */}
          <div className="avatar-upload">
            {imagePreviewUrl ? (
              <img src={imagePreviewUrl} alt="Foreground" className="avatar-preview" />
            ) : (
              <div className="avatar-preview">{name ? name.charAt(0).toUpperCase() : '🏢'}</div>
            )}
            <div className="avatar-actions">
              <strong style={{ fontSize: 13, color: '#1e293b' }}>Foreground Branding Image</strong>
              <small className="muted">Appears prominently at the top of your public profile page.</small>
              <div style={{ display: 'flex', gap: 8, marginTop: 4 }}>
                <input
                  type="file"
                  ref={fileInputRef}
                  style={{ display: 'none' }}
                  accept="image/png,image/jpeg,image/webp,image/svg+xml"
                  onChange={handleImageFileChange}
                />
                <button
                  type="button"
                  className="btn-secondary"
                  disabled={uploadingImage}
                  onClick={() => fileInputRef.current?.click()}
                >
                  {uploadingImage ? 'Uploading…' : imagePreviewUrl ? 'Change image' : 'Upload image'}
                </button>
                {imagePreviewUrl && (
                  <button type="button" className="btn-danger" onClick={handleRemoveImage}>
                    Remove
                  </button>
                )}
              </div>
            </div>
          </div>

          {/* Name Field */}
          <div className="form-group">
            <label>Legal Name / Organization Name</label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Tata Steel Industrial Div."
              required
            />
            <small className="muted">Must be globally unique. Cannot match any existing tenant.</small>
          </div>

          {/* About / Description */}
          <div className="form-group">
            <label>About / Company Background</label>
            <textarea
              rows={4}
              value={about}
              onChange={(e) => setAbout(e.target.value)}
              placeholder="Describe your enterprise, procurement scope, operational domains, or supplier qualifications..."
            />
            <small className="muted">Displayed publicly on your organization profile page.</small>
          </div>

          {/* Read-only Metadata */}
          <div className="form-row">
            <div className="form-group">
              <label>Account Role</label>
              <input type="text" value={profile?.role || ''} readOnly style={{ background: '#f8fafc', color: '#64748b' }} />
            </div>
            <div className="form-group">
              <label>Email Address</label>
              <input
                type="text"
                value={profile?.emailAddress || ''}
                readOnly
                style={{ background: '#f8fafc', color: '#64748b' }}
              />
            </div>
          </div>

          <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: 12 }}>
            <button type="submit" className="primary" disabled={saving || uploadingImage}>
              {saving ? 'Saving changes…' : 'Save Profile Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default App
