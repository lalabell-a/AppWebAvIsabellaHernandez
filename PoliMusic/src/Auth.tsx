import { useState } from 'react';
import { Link } from 'react-router-dom';
import { LoaderCircle, ArrowRight } from 'lucide-react';
import { catalogService, type Profile } from './data';
export default function Auth({ mode, onSuccess }: {
    mode: string;
    onSuccess: (p: Profile) => void;
}) { const [error, setError] = useState(''), [busy, setBusy] = useState(false); return <div className="auth-layout"><div><span className="eyebrow">WELCOME TO YOUR LITTLE UNIVERSE</span><h1>Un lugar al que<br />siempre <em>volver.</em></h1><p>Tu música te estaba esperando.</p><img src="/media/relax.jpg" alt="Paisaje al amanecer"/></div><form onSubmit={async (e) => { e.preventDefault(); setBusy(true); setError(''); const f = Object.fromEntries(new FormData(e.currentTarget)); try {
    onSuccess(await catalogService.auth(mode, f));
}
catch (err) {
    setError((err as Error).message);
}
finally {
    setBusy(false);
} }}><h2>{mode === 'register' ? 'Empieza tu universo.' : 'Qué bueno que volviste.'}</h2><p>{mode === 'register' ? 'Crea una cuenta para guardar tu música.' : 'Inicia sesión y encuentra tu lugar.'}</p><label>Nombre de usuario<input autoComplete="username" name="username" required maxLength={50}/></label>{mode === 'register' && <label>Correo electrónico<input autoComplete="email" type="email" name="email" maxLength={50} required/></label>}<label>Contraseña<input autoComplete={mode === 'register' ? 'new-password' : 'current-password'} type="password" name="password" required minLength={mode === 'register' ? 12 : 1} maxLength={128}/></label>{mode === 'register' && <small>Utiliza al menos 12 caracteres.</small>}{error && <p className="form-error" role="alert">{error}</p>}<button disabled={busy} className="primary">{busy ? <LoaderCircle className="spin" size={18}/> : null}{mode === 'register' ? 'Crear mi cuenta' : 'Entrar a mi universo'}<ArrowRight size={17}/></button><Link className="auth-switch" to={mode === 'register' ? '/login' : '/register'}>{mode === 'register' ? 'Ya tengo una cuenta' : 'Crear una cuenta'}</Link><Link className="auth-switch" to="/">Explorar la demostración sin cuenta</Link></form></div>; }
