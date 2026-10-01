import { useEffect, useMemo, useState } from 'react'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import './App.css'

type Auction = {
  id: number
  docNoYearly: string
  docDate: string
  auctionIntentSubmissionDate?: string
  organization?: { name?: string }
  auctionRequirements?: Requirement[]
  isForwardAuction?: boolean
  auctionStartTime?: string
  auctionEndTime?: string
  statusName?: string
}

type Requirement = {
  id: number
  lineNo: number
  item?: { itemName?: string; name?: string }
  unit?: { alias?: string; name?: string }
  quantity: number
  technicalSpecification?: string
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

const API = import.meta.env.VITE_API_URL || 'http://localhost:5000'
const money = new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 2 })

async function api<T>(path: string, token: string, options: RequestInit = {}) {
  const response = await fetch(`${API}${path}`, {
    ...options,
    headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}`, ...(options.headers || {}) },
  })
  if (!response.ok) throw new Error(await response.text() || `Request failed: ${response.status}`)
  return response.status === 204 ? undefined as T : response.json() as Promise<T>
}

function statusTone(status = '') {
  const s = status.toLowerCase()
  return s.includes('active') ? 'live' : s.includes('close') ? 'closed' : 'scheduled'
}

function App() {
  const [token, setToken] = useState(() => localStorage.getItem('bidnexus_token') || '')
  const [auctions, setAuctions] = useState<Auction[]>([])
  const [selected, setSelected] = useState<Auction | null>(null)
  const [bids, setBids] = useState<Bid[]>([])
  const [statement, setStatement] = useState<Statement[]>([])
  const [bidAmount, setBidAmount] = useState('')
  const [loading, setLoading] = useState(false)
  const [notice, setNotice] = useState('')
  const [search, setSearch] = useState('')
  const [realtime, setRealtime] = useState(false)

  useEffect(() => {
    if (!token) return
    localStorage.setItem('bidnexus_token', token)
    loadAuctions()
  }, [token])

  async function loadAuctions() {
    setLoading(true)
    try {
      setAuctions(await api<Auction[]>('/api/auctions?pageNo=1&pageSize=50', token))
    } catch (e) { setNotice(e instanceof Error ? e.message : 'Unable to load auctions') }
    finally { setLoading(false) }
  }

  async function openAuction(auction: Auction) {
    setSelected(auction)
    setStatement([])
    setNotice('')
    try {
      const [detail, leaderboard] = await Promise.all([
        api<Auction>(`/api/auctions/${auction.id}`, token),
        api<Bid[]>(`/api/bids/auction/${auction.id}/leaderboard`, token),
      ])
      setSelected(detail)
      setBids(leaderboard)
    } catch (e) { setNotice(e instanceof Error ? e.message : 'Unable to load auction') }
  }

  async function submitBid() {
    if (!selected || !bidAmount) return
    const amount = Number(bidAmount)
    if (!Number.isFinite(amount) || amount <= 0) return setNotice('Enter a valid bid amount.')
    const details = (selected.auctionRequirements || []).map(r => ({
      auctionRequirementId: r.id, rate: amount / Math.max(r.quantity, 1), baseAmount: amount, netAmount: amount, taxes: [],
    }))
    try {
      const result = await api<Bid>('/api/bids', token, {
        method: 'POST',
        body: JSON.stringify({
          auctionId: selected.id, vendorId: 0, basicAmount: amount, taxAmount: 0,
          discountAmount: 0, netAmount: amount, mainBidId: null, bidRevisionNo: 0, bidDetails: details,
        }),
      })
      setBids(current => [{ ...result, isCurrent: true }, ...current])
      setBidAmount('')
      setNotice('Bid accepted. Other participants have been notified live.')
    } catch (e) { setNotice(e instanceof Error ? e.message : 'Bid could not be submitted') }
  }

  async function loadStatement() {
    if (!selected) return
    try {
      setStatement(await api<Statement[]>(`/api/auctions/${selected.id}/statement`, token))
    } catch (e) { setNotice(e instanceof Error ? e.message : 'Statement unavailable') }
  }

  useEffect(() => {
    if (!selected || !token) return

    const connection = new HubConnectionBuilder()
      .withUrl(`${API}/hubs/auction`, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('BidAccepted', (event: { AuctionId: number; BidId: number; VendorId: number; NetAmount: number }) => {
      const liveBid: Bid = { id: event.BidId, vendorId: event.VendorId, netAmount: event.NetAmount, basicAmount: event.NetAmount, taxAmount: 0, createdAt: new Date().toISOString(), isCurrent: true, bidRevisionNo: 0 }
      setBids(current => [liveBid, ...current.filter(b => b.id !== event.BidId)])
      setNotice('A new bid was accepted.')
    })
    connection.on('AuctionClosed', () => {
      setSelected(current => current ? { ...current, statusName: 'Closed' } : current)
      setNotice('Auction closed. Final statement is now available to the organization.')
    })

    connection.start()
      .then(() => connection.invoke('JoinAuction', selected.id))
      .then(() => setRealtime(true))
      .catch(() => setRealtime(false))

    return () => {
      setRealtime(false)
      connection.invoke('LeaveAuction', selected.id).catch(() => undefined)
      connection.stop()
    }
  }, [selected?.id, token])

  const filtered = useMemo(() => auctions.filter(a =>
    `${a.docNoYearly} ${a.organization?.name || ''}`.toLowerCase().includes(search.toLowerCase())
  ), [auctions, search])

  if (!token) return (
    <main className="auth-shell">
      <div className="auth-card">
        <div className="brand"><span className="brand-mark">B</span><span>Bid<span>Nexus</span></span></div>
        <p className="eyebrow">AUCTION OPERATIONS</p>
        <h1>Enter the command center.</h1>
        <p className="muted">Paste a JWT from the BidNexus API to connect this frontend to your live environment.</p>
        <label>API token</label>
        <textarea value={token} onChange={e => setToken(e.target.value.trim())} placeholder="eyJhbGciOi..." />
        <button className="primary" disabled={!token.trim()} onClick={() => setToken(token.trim())}>Connect to BidNexus</button>
      </div>
    </main>
  )

  return (
    <div className="app-shell">
      <aside>
        <div className="brand"><span className="brand-mark">B</span><span>Bid<span>Nexus</span></span></div>
        <nav>
          <button className="nav-active">Overview</button>
          <button onClick={loadAuctions}>Auctions</button>
          <button onClick={() => selected && loadStatement()}>Statements</button>
        </nav>
        <div className="side-bottom">
          <div className="connection"><i /> API connected</div>
          <button className="logout" onClick={() => { localStorage.removeItem('bidnexus_token'); setToken('') }}>Disconnect</button>
        </div>
      </aside>

      <main className="workspace">
        <header>
          <div>
            <p className="eyebrow">PROCUREMENT CONTROL</p>
            <h1>Live auction desk</h1>
          </div>
          <div className="header-actions"><span className="live-dot" />{realtime ? "Realtime connected" : "Realtime connecting"} <button onClick={loadAuctions}>↻</button></div>
        </header>

        {notice && <div className="notice">{notice}<button onClick={() => setNotice('')}>×</button></div>}

        {!selected ? (
          <>
            <section className="stats">
              <div><span>Visible auctions</span><strong>{auctions.length}</strong></div>
              <div><span>Live now</span><strong>{auctions.filter(a => statusTone(a.statusName) === 'live').length}</strong></div>
              <div><span>Scheduled</span><strong>{auctions.filter(a => statusTone(a.statusName) === 'scheduled').length}</strong></div>
              <div><span>Closed</span><strong>{auctions.filter(a => statusTone(a.statusName) === 'closed').length}</strong></div>
            </section>
            <div className="section-head"><div><p className="eyebrow">AUCTION REGISTER</p><h2>Opportunities</h2></div><input value={search} onChange={e => setSearch(e.target.value)} placeholder="Search auction..." /></div>
            <section className="auction-grid">
              {loading ? <div className="empty">Loading auctions…</div> : filtered.map(a => (
                <button className="auction-card" key={a.id} onClick={() => openAuction(a)}>
                  <div className="card-top"><span className={`status ${statusTone(a.statusName)}`}>{a.statusName || 'Scheduled'}</span><span>#{a.id}</span></div>
                  <h3>{a.docNoYearly || `Auction ${a.id}`}</h3>
                  <p>{a.organization?.name || 'Organization auction'}</p>
                  <div className="card-foot"><span>{a.isForwardAuction ? 'Forward' : 'Reverse'} auction</span><span>View →</span></div>
                </button>
              ))}
              {!loading && filtered.length === 0 && <div className="empty">No auctions match your search.</div>}
            </section>
          </>
        ) : (
          <AuctionDesk auction={selected} bids={bids} bidAmount={bidAmount} setBidAmount={setBidAmount} submitBid={submitBid} statement={statement} loadStatement={loadStatement} />
        )}
      </main>
    </div>
  )
}

function AuctionDesk({ auction, bids, bidAmount, setBidAmount, submitBid, statement, loadStatement }: {
  auction: Auction; bids: Bid[]; bidAmount: string; setBidAmount: (v: string) => void; submitBid: () => void; statement: Statement[]; loadStatement: () => void
}) {
  const best = bids.length ? bids[0].netAmount : null
  return <div className="desk">
    <button className="back" onClick={() => location.reload()}>← All auctions</button>
    <div className="auction-hero">
      <div><p className="eyebrow">{auction.isForwardAuction ? 'FORWARD AUCTION' : 'REVERSE AUCTION'} · #{auction.id}</p><h1>{auction.docNoYearly || 'Auction'}</h1><p className="muted">{auction.organization?.name || 'Organization'} · {auction.auctionRequirements?.length || 0} requirements</p></div>
      <span className={`status large ${statusTone(auction.statusName)}`}>{auction.statusName || 'Scheduled'}</span>
    </div>
    <div className="desk-grid">
      <section className="panel bid-panel">
        <div className="panel-head"><div><p className="eyebrow">YOUR BID</p><h2>{auction.isForwardAuction ? 'Submit your offer' : 'Submit your lowest offer'}</h2></div><span className="live-pill"><i /> LIVE</span></div>
        <div className="amount-wrap"><span>₹</span><input type="number" value={bidAmount} onChange={e => setBidAmount(e.target.value)} placeholder="0.00" /><small>Net amount</small></div>
        {best !== null && <div className="best-row"><span>Current market position</span><strong>{money.format(best)}</strong></div>}
        <button className="primary wide" onClick={submitBid}>Submit bid <span>↗</span></button>
        <p className="hint">Server calculates quantity, taxes and final net amount. Client values are treated as preview only.</p>
      </section>
      <section className="panel">
        <div className="panel-head"><div><p className="eyebrow">LIVE MARKET</p><h2>Leaderboard</h2></div><span className="live-pill"><i /> LIVE</span></div>
        <div className="leaderboard">{bids.slice(0, 8).map((b, i) => <div className={`leader ${i === 0 ? 'leader-first' : ''}`} key={b.id}><b>{String(i + 1).padStart(2, '0')}</b><span>{b.vendor?.name || `Vendor #${b.vendorId}`}</span><strong>{money.format(b.netAmount)}</strong></div>)}{!bids.length && <div className="empty">No bids yet.</div>}</div>
      </section>
    </div>
    <section className="panel requirements"><div className="panel-head"><div><p className="eyebrow">SCOPE</p><h2>Requirements</h2></div></div>{(auction.auctionRequirements || []).map(r => <div className="requirement" key={r.id}><span>{String(r.lineNo).padStart(2, '0')}</span><div><strong>{r.item?.itemName || r.item?.name || `Requirement #${r.id}`}</strong><small>{r.technicalSpecification || 'No technical specification provided'}</small></div><b>{r.quantity} {r.unit?.alias || r.unit?.name || ''}</b></div>)}</section>
    <section className="panel statement"><div className="panel-head"><div><p className="eyebrow">FINAL RESULT</p><h2>Auction statement</h2></div><button onClick={loadStatement}>Load statement</button></div>{statement.length ? statement.map(s => <div className="winner"><div className="winner-icon">✓</div><div><p className="eyebrow">WINNER</p><h3>{s.vendorName}</h3><span>Bid #{s.bidId} · Rank {s.rank}</span></div><strong>{money.format(s.netAmount)}</strong></div>) : <p className="muted statement-empty">Statement becomes available after the auction closes and is visible to the organization.</p>}</section>
  </div>
}

export default App
