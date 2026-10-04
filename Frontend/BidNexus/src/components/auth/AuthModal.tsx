import React, { useState } from 'react';
import { KeyRound, User, Lock, Building, Store, ArrowRight, ShieldCheck, Mail, Phone } from 'lucide-react';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { api } from '../../services/api';

interface AuthModalProps {
  onConnectToken: (token: string, refreshToken?: string, expiresAt?: string, refreshExpiresAt?: string) => void;
  sessionNotice?: string;
}

export const AuthModal: React.FC<AuthModalProps> = ({ onConnectToken, sessionNotice }) => {
  const [tab, setTab] = useState<'token' | 'login' | 'register'>('login');
  const [tokenInput, setTokenInput] = useState('');
  
  // Login fields
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  // Register fields
  const [regName, setRegName] = useState('');
  const [regUsername, setRegUsername] = useState('');
  const [regEmail, setRegEmail] = useState('');
  const [regContactNumber, setRegContactNumber] = useState('');
  const [regPassword, setRegPassword] = useState('');
  const [isOrganization, setIsOrganization] = useState(false);
  const [officialAddress, setOfficialAddress] = useState('');
  const [about, setAbout] = useState('');

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [successMsg, setSuccessMsg] = useState('');

  const handleTokenSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!tokenInput.trim()) {
      setError('Please paste a valid JWT access token.');
      return;
    }
    setError('');
    onConnectToken(tokenInput.trim());
  };

  const handleLoginSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!username.trim() || !password.trim()) {
      setError('Please enter both username and password.');
      return;
    }
    setError('');
    setLoading(true);
    try {
      const res = await api.login(username.trim(), password);
      const token = res.accessToken || res.token;
      if (token) {
        onConnectToken(token, res.refreshToken, res.expiresAt, res.refreshTokenExpiresAt);
      } else {
        throw new Error('Authentication succeeded but no access token was returned.');
      }
    } catch (err: any) {
      setError(err instanceof Error ? err.message : 'Invalid credentials or login failed.');
    } finally {
      setLoading(false);
    }
  };

  const handleRegisterSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!regUsername.trim() || !regPassword.trim() || !regEmail.trim()) {
      setError('Please fill in required username, email, and password.');
      return;
    }
    setError('');
    setLoading(true);
    try {
      await api.register({
        name: regName.trim() || regUsername.trim(),
        contactNumber: regContactNumber.trim() || 'N/A',
        emailAddress: regEmail.trim(),
        userName: regUsername.trim(),
        password: regPassword,
        registerAsOrganization: isOrganization,
        officialAddress: officialAddress.trim(),
        foregroundImageId: null,
        about: about.trim(),
      });
      setSuccessMsg('Registration successful! You can now log in with your credentials.');
      setTab('login');
      setUsername(regUsername);
      setPassword(regPassword);
    } catch (err: any) {
      setError(err instanceof Error ? err.message : 'Registration failed.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="bn-auth-shell">
      <div className="bn-auth-card">
        <div className="bn-auth-brand">
          <div className="bn-brand-logo">
            <span>B</span>
          </div>
          <div>
            <h2 className="bn-brand-name">Bid<span className="bn-brand-accent">Nexus</span></h2>
            <p className="bn-brand-tagline">B2B Procurement & Real-time Auction Workstation</p>
          </div>
        </div>

        <div className="bn-auth-tabs">
          <button
            className={`bn-auth-tab ${tab === 'login' ? 'active' : ''}`}
            onClick={() => { setTab('login'); setError(''); setSuccessMsg(''); }}
          >
            Sign In
          </button>
          <button
            className={`bn-auth-tab ${tab === 'register' ? 'active' : ''}`}
            onClick={() => { setTab('register'); setError(''); setSuccessMsg(''); }}
          >
            Register Entity
          </button>
          <button
            className={`bn-auth-tab ${tab === 'token' ? 'active' : ''}`}
            onClick={() => { setTab('token'); setError(''); setSuccessMsg(''); }}
          >
            JWT Token Session
          </button>
        </div>

        {sessionNotice && (
          <div
            className="bn-auth-notice"
            style={{
              background: 'rgba(234, 179, 8, 0.12)',
              border: '1px solid rgba(234, 179, 8, 0.3)',
              color: '#fbbf24',
              padding: '10px 14px',
              borderRadius: '8px',
              fontSize: '13px',
              marginBottom: '16px',
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
            }}
          >
            <Lock size={16} />
            <span>{sessionNotice}</span>
          </div>
        )}
        {error && <div className="bn-auth-error">{error}</div>}
        {successMsg && <div className="bn-auth-success">{successMsg}</div>}

        {tab === 'login' && (
          <form onSubmit={handleLoginSubmit} className="bn-auth-form">
            <Input
              label="Username / ID"
              placeholder="Enter your organization or vendor username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              leftIcon={<User size={16} />}
              required
            />
            <Input
              label="Password"
              type="password"
              placeholder="Enter your security password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              leftIcon={<Lock size={16} />}
              required
            />

            <Button
              type="submit"
              variant="primary"
              size="lg"
              className="bn-w-full bn-mt-2"
              loading={loading}
              icon={<ArrowRight size={18} />}
            >
              Authenticate & Enter Desk
            </Button>
          </form>
        )}

        {tab === 'register' && (
          <form onSubmit={handleRegisterSubmit} className="bn-auth-form">
            <div className="bn-grid-2">
              <Input
                label="Full / Legal Name"
                placeholder="Entity name or Representative name"
                value={regName}
                onChange={(e) => setRegName(e.target.value)}
                leftIcon={<User size={16} />}
              />
              <Input
                label="Email Address"
                type="email"
                placeholder="procurement@company.com"
                value={regEmail}
                onChange={(e) => setRegEmail(e.target.value)}
                leftIcon={<Mail size={16} />}
                required
              />
            </div>

            <div className="bn-grid-2">
              <Input
                label="Contact / Mobile"
                type="tel"
                placeholder="+1 555-0199 or 9876543210"
                value={regContactNumber}
                onChange={(e) => setRegContactNumber(e.target.value)}
                leftIcon={<Phone size={16} />}
              />
              <Input
                label="Username"
                placeholder="Unique login handle"
                value={regUsername}
                onChange={(e) => setRegUsername(e.target.value)}
                leftIcon={<User size={16} />}
                required
              />
            </div>
            <Input
              label="Password"
              type="password"
              placeholder="Secure password"
              value={regPassword}
              onChange={(e) => setRegPassword(e.target.value)}
              leftIcon={<Lock size={16} />}
              required
            />

            <div className="bn-field">
              <label className="bn-label">Entity Type</label>
              <div className="bn-role-selector">
                <button
                  type="button"
                  className={`bn-role-opt ${!isOrganization ? 'selected' : ''}`}
                  onClick={() => setIsOrganization(false)}
                >
                  <Store size={18} />
                  <div>
                    <strong>Vendor</strong>
                    <small>Participate in auctions & submit bids</small>
                  </div>
                </button>
                <button
                  type="button"
                  className={`bn-role-opt ${isOrganization ? 'selected' : ''}`}
                  onClick={() => setIsOrganization(true)}
                >
                  <Building size={18} />
                  <div>
                    <strong>Organization</strong>
                    <small>Create procurement auctions & issue awards</small>
                  </div>
                </button>
              </div>
            </div>

            <Input
              label="Official Address"
              placeholder="Corporate headquarters or business location"
              value={officialAddress}
              onChange={(e) => setOfficialAddress(e.target.value)}
            />
            <Input
              label="About / Profile Summary"
              placeholder="Brief overview of entity business scope"
              value={about}
              onChange={(e) => setAbout(e.target.value)}
            />

            <Button
              type="submit"
              variant="primary"
              size="lg"
              className="bn-w-full bn-mt-2"
              loading={loading}
              icon={<ArrowRight size={18} />}
            >
              Register & Create Tenant
            </Button>
          </form>
        )}

        {tab === 'token' && (
          <form onSubmit={handleTokenSubmit} className="bn-auth-form">
            <div className="bn-field">
              <label className="bn-label">Paste Access Token (JWT)</label>
              <textarea
                className="bn-textarea bn-token-textarea"
                rows={5}
                placeholder="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                value={tokenInput}
                onChange={(e) => setTokenInput(e.target.value)}
              />
              <p className="bn-hint-msg">
                Direct token session allows mounting your authenticated tenant workspace directly into this browser workstation.
              </p>
            </div>

            <Button
              type="submit"
              variant="primary"
              size="lg"
              className="bn-w-full bn-mt-2"
              icon={<KeyRound size={18} />}
            >
              Connect Session Token
            </Button>
          </form>
        )}

        <div className="bn-auth-footer">
          <ShieldCheck size={14} />
          <span>Enterprise-grade JWT Authentication with Automated Tenant Scope Isolation</span>
        </div>
      </div>
    </main>
  );
};
