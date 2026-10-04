import { useState, type FormEvent } from 'react';
import { api } from '../../services/api';
import { errorMessage } from '../../lib/appContext';
import { Button } from '../../components/ui/Button';
import { TextField, TextAreaField } from '../../components/ui/Field';
import { Callout } from '../../components/ui/States';

interface AuthScreenProps {
  notice?: string;
  onSignedIn: (accessToken: string, refreshToken?: string, refreshExpiresAt?: string) => void;
}

type Mode = 'signin' | 'register';

export function AuthScreen({ notice, onSignedIn }: AuthScreenProps) {
  const [mode, setMode] = useState<Mode>('signin');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [info, setInfo] = useState(notice || '');

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const [asOrg, setAsOrg] = useState(false);
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [about, setAbout] = useState('');

  const switchMode = (m: Mode) => {
    setMode(m);
    setError('');
    setInfo('');
  };

  const signIn = async (e: FormEvent) => {
    e.preventDefault();
    if (!username.trim() || !password) {
      setError('Enter your username and password.');
      return;
    }
    setBusy(true);
    setError('');
    try {
      const res = await api.login(username.trim(), password);
      const token = res.accessToken || res.token;
      if (!token) throw new Error('Sign-in succeeded but no access token was returned.');
      onSignedIn(token, res.refreshToken, res.refreshTokenExpiresAt);
    } catch (err) {
      setError(errorMessage(err, 'Incorrect username or password.'));
    } finally {
      setBusy(false);
    }
  };

  const register = async (e: FormEvent) => {
    e.preventDefault();
    if (!name.trim() || !username.trim() || !email.trim() || !password) {
      setError('Company name, username, email and password are required.');
      return;
    }
    if (password.length < 8) {
      setError('Use at least 8 characters for the password.');
      return;
    }
    setBusy(true);
    setError('');
    try {
      await api.register({
        name: name.trim(),
        contactNumber: phone.trim(),
        emailAddress: email.trim(),
        userName: username.trim(),
        password,
        registerAsOrganization: asOrg,
        officialAddress: address.trim(),
        foregroundImageId: null,
        about: about.trim(),
      });
      setMode('signin');
      setPassword('');
      setInfo('Account created. Sign in to continue.');
    } catch (err) {
      setError(errorMessage(err, 'Registration failed.'));
    } finally {
      setBusy(false);
    }
  };

  return (
    <main className="auth">
      <div className={`card auth__card${mode === 'register' ? ' auth__card--wide' : ''}`}>
        <div className="stack-sm">
          <span className="brand" style={{ padding: 0 }}>
            <span className="brand__mark" aria-hidden="true">B</span>
            BidNexus
          </span>
          <h1 style={{ fontSize: 'var(--text-xl)' }}>{mode === 'signin' ? 'Sign in' : 'Create an account'}</h1>
          <p className="muted small">
            {mode === 'signin' ? 'Procurement auctions for buyers and suppliers.' : 'Choose how your company will use BidNexus.'}
          </p>
        </div>

        {info && <Callout tone="info">{info}</Callout>}
        {error && <Callout tone="error">{error}</Callout>}

        {mode === 'signin' ? (
          <form className="stack" onSubmit={signIn} noValidate>
            <TextField label="Username" autoComplete="username" value={username} onChange={(e) => setUsername(e.target.value)} />
            <TextField label="Password" type="password" autoComplete="current-password" value={password} onChange={(e) => setPassword(e.target.value)} />
            <Button type="submit" variant="primary" size="lg" block loading={busy}>
              Sign in
            </Button>
            <p className="small muted" style={{ textAlign: 'center' }}>
              New to BidNexus?{' '}
              <button type="button" className="link-btn accent" onClick={() => switchMode('register')}>
                Create an account
              </button>
            </p>
          </form>
        ) : (
          <form className="stack" onSubmit={register} noValidate>
            <div className="segmented" role="group" aria-label="Account type">
              <button type="button" className="segmented__option" aria-pressed={!asOrg} onClick={() => setAsOrg(false)}>
                <span className="strong">Vendor</span>
                <span className="small muted">I bid in auctions</span>
              </button>
              <button type="button" className="segmented__option" aria-pressed={asOrg} onClick={() => setAsOrg(true)}>
                <span className="strong">Buyer organisation</span>
                <span className="small muted">I run auctions</span>
              </button>
            </div>
            <div className="form-grid">
              <TextField label="Company name" className="span-2" value={name} onChange={(e) => setName(e.target.value)} />
              <TextField label="Work email" type="email" autoComplete="email" value={email} onChange={(e) => setEmail(e.target.value)} />
              <TextField label="Phone" type="tel" autoComplete="tel" value={phone} onChange={(e) => setPhone(e.target.value)} />
              <TextField label="Username" autoComplete="username" value={username} onChange={(e) => setUsername(e.target.value)} />
              <TextField
                label="Password"
                type="password"
                autoComplete="new-password"
                hint="At least 8 characters"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              {asOrg && <TextField label="Registered address" className="span-2" value={address} onChange={(e) => setAddress(e.target.value)} />}
              <TextAreaField label="About the company (optional)" className="span-2" value={about} onChange={(e) => setAbout(e.target.value)} />
            </div>
            <Button type="submit" variant="primary" size="lg" block loading={busy}>
              Create account
            </Button>
            <p className="small muted" style={{ textAlign: 'center' }}>
              Already registered?{' '}
              <button type="button" className="link-btn accent" onClick={() => switchMode('signin')}>
                Sign in
              </button>
            </p>
          </form>
        )}
      </div>
    </main>
  );
}
