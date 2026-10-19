import Modal from './Modal';
import Auth from './Auth';
import Admin from './Admin';
import { useEffect, useRef, useState, type FormEvent, type ReactNode } from 'react';
import { Link, NavLink, useLocation, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { motion } from 'motion/react';
import { AudioLines, House, Compass, Library, Plus, Search, ArrowUpRight, ArrowRight, ChevronLeft, ChevronRight, Play, Music2, Disc3, Heart, Settings, Sun, Moon, X, Check, MoreHorizontal, Headphones, ListMusic, LogOut, Lock, Globe, Trash2, Upload, Shield, LoaderCircle, AlertCircle } from 'lucide-react';
import { albums as seedAlbums, artists as seedArtists, songs as seedSongs, catalogService, initialPlaylists, initialProfile, readLocal, apiBase, request, type Song, type Playlist, type Profile } from './data';
import Player from './Player';
function Cover({ kind = 'rock', title = '', small = false }: {
    kind?: string;
    title?: string;
    small?: boolean;
}) { return <div className={`cover cover-${kind} ${small ? 'small' : ''}`}><img src={`/media/${kind}.jpg`} alt="" loading="lazy"/><div className="cover-veil"/><span className="cover-edition">POLIMUSIC / SELECTED SOUNDS</span><span className="cover-title">{title}</span><span className="cover-bottom">A LITTLE WORLD OF YOUR OWN <span>↗</span></span></div>; }
export default function App() {
    const location = useLocation(), navigate = useNavigate();
    const path = location.pathname;
    const { data, isPending, isError, refetch } = useQuery({ queryKey: ['catalog'], queryFn: catalogService.load, retry: 1 });
    const songs = data?.songs || seedSongs, albums = data?.albums || seedAlbums, artists = data?.artists || seedArtists;
    const [playlists, setPlaylists] = useState<Playlist[]>(() => apiBase ? [] : readLocal('pm-playlists', initialPlaylists));
    const [profile, setProfile] = useState<Profile>(() => apiBase ? { id: 0, username: 'Invitado', email: '', birthday: '', photo: '', role: 0 } : readLocal('pm-profile', initialProfile));
    const [theme, setTheme] = useState(() => readLocal('pm-theme', 'dark'));
    const [query, setQuery] = useState(''), [filter, setFilter] = useState('Todo'), [sort, setSort] = useState('default');
    const [modal, setModal] = useState(''), [toast, setToast] = useState('');
    const [edit, setEdit] = useState<Playlist | null>(null);
    const [targetSong, setTargetSong] = useState<Song | null>(null);
    const [current, setCurrent] = useState<Song | null>(null), [source, setSource] = useState('');
    const [queue, setQueue] = useState<number[]>([1, 2, 3, 4, 5]);
    const [uploads, setUploads] = useState<Record<number, string>>({});
    const receipt = useRef('');
    const [session, setSession] = useState(false), [localPlays, setLocalPlays] = useState<Record<number, number>>({});
    useEffect(() => { document.documentElement.dataset.theme = theme; localStorage.setItem('pm-theme', JSON.stringify(theme)); }, [theme]);
    useEffect(() => { if (!apiBase)
        localStorage.setItem('pm-playlists', JSON.stringify(playlists)); }, [playlists]);
    useEffect(() => { if (!apiBase)
        localStorage.setItem('pm-profile', JSON.stringify(profile)); }, [profile]);
    useEffect(() => { if (!toast)
        return; const id = setTimeout(() => setToast(''), 3500); return () => clearTimeout(id); }, [toast]);
    useEffect(() => { setFilter('Todo'); window.scrollTo(0, 0); }, [path]);
    useEffect(() => { if (apiBase)
        request<Profile>('/api/auth/me').then(p => { setProfile(p); setSession(true); }).catch(() => { }); }, []);
    useEffect(() => { if (!apiBase || !session)
        return; request<Playlist[]>('/api/playlists').then(setPlaylists).catch(e => setToast(e.message)); }, [session]);
    const artistNames = (s: Song) => s.artistIds.map(id => artists.find(a => a.id === id)?.name).filter(Boolean).join(', ');
    function notify(s: string) { setToast(s); }
    function play(song: Song, ids?: number[]) { if (ids)
        setQueue(ids); setCurrent(song); const src = uploads[song.id] || (apiBase ? `${apiBase}/api/media/song/${song.id}` : song.id === 1 ? '/resources/songfiles/bensound-adventure.mp3' : ''); setSource(src); if (!src)
        setModal('audio'); }
    function step(delta: number) { const at = queue.indexOf(current?.id || 0); const next = songs.find(s => s.id === queue[(at + delta + queue.length) % queue.length]); if (next)
        play(next); }
    function newPlaylist() { setEdit(null); setModal('playlist'); }
    async function savePlaylist(e: FormEvent<HTMLFormElement>) { e.preventDefault(); const f = new FormData(e.currentTarget); const title = String(f.get('title')).trim(); if (!title)
        return; const value: Playlist = { id: edit?.id || Date.now(), userId: profile.id, title, type: +String(f.get('type')), photo: String(f.get('photo')), created: edit?.created || new Date().toISOString(), songIds: edit?.songIds || [] }; try {
        const saved = apiBase ? await request<Playlist>(`/api/playlists${edit ? `/${edit.id}` : ''}`, { method: edit ? 'PUT' : 'POST', body: JSON.stringify(value) }) : value;
        setPlaylists(p => edit ? p.map(x => x.id === edit.id ? saved : x) : [...p, saved]);
        setModal('');
        notify(edit ? 'Playlist actualizada' : 'Tu nuevo universo está listo');
        navigate(`/playlist/${saved.id}`);
    }
    catch (e) {
        notify((e as Error).message);
    } }
    async function deletePlaylist(p: Playlist) { try {
        if (apiBase)
            await request(`/api/playlists/${p.id}`, { method: 'DELETE' });
        setPlaylists(v => v.filter(x => x.id !== p.id));
        setModal('');
        navigate('/playlists');
        notify('Playlist eliminada');
    }
    catch (e) {
        notify((e as Error).message);
    } }
    async function addSong(p: Playlist, s: Song) { if (p.songIds.includes(s.id)) {
        notify('Esta canción ya está en la playlist');
        return;
    } try {
        if (apiBase)
            await request(`/api/playlists/${p.id}/songs/${s.id}`, { method: 'POST' });
        setPlaylists(v => v.map(x => x.id === p.id ? { ...x, songIds: [...x.songIds, s.id] } : x));
        setModal('');
        notify(`Añadida a ${p.title}`);
    }
    catch (e) {
        notify((e as Error).message);
    } }
    async function removeSong(p: Playlist, s: Song) { try {
        if (apiBase)
            await request(`/api/playlists/${p.id}/songs/${s.id}`, { method: 'DELETE' });
        setPlaylists(v => v.map(x => x.id === p.id ? { ...x, songIds: x.songIds.filter(id => id !== s.id) } : x));
        notify('Canción quitada de la playlist');
    }
    catch (e) {
        notify((e as Error).message);
    } }
    const filtered = songs.filter(s => `${s.name} ${artistNames(s)}`.toLowerCase().includes(query.toLowerCase())).sort((a, b) => sort === 'name' ? a.name.localeCompare(b.name) : sort === 'plays' ? b.plays - a.plays : 0);
    function rows(items: Song[], p?: Playlist) { return <div className="song-table"><div className="song-header"><span>#</span><span>Canción / artista</span><span>Álbum</span><span>Reproducciones</span><span /></div>{items.map((s, i) => <div className={`song-row ${current?.id === s.id ? 'selected' : ''}`} key={s.id}><button className="row-play" aria-label={`Reproducir ${s.name}`} onClick={() => play(s, items.map(x => x.id))}><span>{String(i + 1).padStart(2, '0')}</span><Play size={15}/></button><button className="track" onClick={() => play(s, items.map(x => x.id))}><img src={`/media/${albums.find(a => a.songIds.includes(s.id))?.cover || 'rock'}.jpg`} alt=""/><span><strong>{s.name}</strong><small>{artistNames(s)}</small></span></button><Link className="album-cell" to={`/album/${albums.find(a => a.songIds.includes(s.id))?.id}`}>{albums.find(a => a.songIds.includes(s.id))?.title}</Link><span className="plays">{s.plays + (localPlays[s.id] || 0)}</span>{p ? <button aria-label={`Quitar ${s.name}`} onClick={() => removeSong(p, s)}><X size={17}/></button> : <button aria-label={`Opciones de ${s.name}`} onClick={() => { setTargetSong(s); setModal('song'); }}><MoreHorizontal size={20}/></button>}</div>)}{!items.length && <div className="empty"><Music2 /><h3>Aquí empieza algo bueno.</h3><p>{query ? 'No encontramos canciones con esa búsqueda.' : 'Agrega una canción para darle vida a tu playlist.'}</p><button onClick={() => navigate('/explore')} className="secondary">Explorar canciones <ArrowRight size={16}/></button></div>}</div>; }
    function albumCards() { return <div className="album-grid">{albums.filter(a => a.title.toLowerCase().includes(query.toLowerCase())).map(a => <Link to={`/album/${a.id}`} className="album-card" key={a.id}><div className="art-wrap"><Cover kind={a.cover} title={a.title}/><span className="floating-play"><Play fill="currentColor" size={18}/></span></div><h3>{a.title}</h3><p>{a.year} <span>·</span> {a.songIds.length} canciones</p></Link>)}</div>; }
    function artistCards() { return <div className="artist-grid">{artists.filter(a => a.name.toLowerCase().includes(query.toLowerCase())).map((a, i) => <Link to={`/artist/${a.id}`} className="artist-card" key={a.id}><div><img src={`/media/${albums[i % 5].cover}.jpg`} alt="Arte abstracto de perfil"/><ArrowUpRight size={18}/></div><h3>{a.name}</h3><p>Artista · {songs.filter(s => s.artistIds.includes(a.id)).length} canciones</p></Link>)}</div>; }
    function playlistCards() { return <div className="album-grid playlists-grid">{playlists.filter(p => p.title.toLowerCase().includes(query.toLowerCase())).map((p, i) => <Link to={`/playlist/${p.id}`} className="album-card" key={p.id}><div className="playlist-cover">{p.photo ? <img src={p.photo} alt=""/> : <Cover kind={albums[i % 5].cover} title={p.title}/>}<span className="privacy">{p.type ? <Lock size={14}/> : <Globe size={14}/>}</span></div><h3>{p.title}</h3><p>{p.songIds.length} canciones <span>·</span> {p.type ? 'Privada' : 'Pública'}</p></Link>)}<button className="create-card" onClick={newPlaylist}><Plus /><strong>Un nuevo mood</strong><small>Crea tu playlist</small></button></div>; }
    let content: ReactNode;
    const detailId = +path.split('/')[2];
    if (path.startsWith('/album/')) {
        const a = albums.find(x => x.id === detailId);
        content = a ? <><div className="detail-hero"><Cover kind={a.cover} title={a.title}/><div><span className="eyebrow">EL ÁLBUM / {a.year}</span><h1>{a.title}</h1><p>{a.songIds.length} canciones · Una historia para escuchar</p><button className="primary" onClick={() => play(songs.find(s => s.id === a.songIds[0])!, a.songIds)}><Play size={17} fill="currentColor"/> Reproducir álbum</button></div></div>{rows(songs.filter(s => a.songIds.includes(s.id)))}</> : <Empty />;
    }
    else if (path.startsWith('/artist/')) {
        const a = artists.find(x => x.id === detailId);
        content = a ? <><div className="artist-detail" style={{ backgroundImage: `linear-gradient(90deg,rgba(12,13,16,.9),rgba(12,13,16,.1)),url(/media/${albums[(a.id - 1) % 5].cover}.jpg)` }}><span className="eyebrow">SU MÚSICA, TU UNIVERSO</span><h1>{a.name}</h1><p>Descubre las canciones de este artista en PoliMusic.</p><small>Imagen de ambientación · fotografía original no incluida</small></div><Section title="Sus canciones" label="EL CATÁLOGO"/>{rows(songs.filter(s => s.artistIds.includes(a.id)))}</> : <Empty />;
    }
    else if (path.startsWith('/playlist/')) {
        const p = playlists.find(x => x.id === detailId);
        content = p ? <><div className="detail-hero"><Cover kind={albums[playlists.indexOf(p) % 5]?.cover || 'rock'} title={p.title}/><div><span className="eyebrow">TU PLAYLIST · {p.type ? 'PRIVADA' : 'PÚBLICA'}</span><h1>{p.title}</h1><p>{profile.username} · {p.songIds.length} canciones</p><div className="actions"><button className="primary" disabled={!p.songIds.length} onClick={() => play(songs.find(s => s.id === p.songIds[0])!, p.songIds)}><Play size={17} fill="currentColor"/> Escuchar</button><button className="secondary" onClick={() => { setEdit(p); setModal('playlist'); }}>Editar</button><button className="icon-btn" aria-label="Eliminar playlist" onClick={() => { setEdit(p); setModal('delete'); }}><Trash2 size={18}/></button></div></div></div>{rows(songs.filter(s => p.songIds.includes(s.id)), p)}<button className="secondary" onClick={() => navigate('/explore')}><Plus size={16}/> Agregar canciones</button></> : <Empty />;
    }
    else if (path === '/playlists') {
        content = <><PageTitle eyebrow="HECHAS POR TI" title="Cada playlist, un pequeño mundo." text="Para ese viaje, esa persona, ese momento." action={<button className="primary" onClick={newPlaylist}><Plus size={18}/> Crear playlist</button>}/>{playlistCards()}</>;
    }
    else if (path === '/profile') {
        content = <><PageTitle eyebrow="TU ESPACIO" title="Tan tú como tu música." text="Los pequeños detalles que hacen este universo tuyo."/><form className="profile-form" onSubmit={async (e) => { e.preventDefault(); const f = new FormData(e.currentTarget); const v = { ...profile, username: String(f.get('username')).trim(), email: String(f.get('email')).trim(), birthday: String(f.get('birthday')), photo: String(f.get('photo')) }; try {
            if (apiBase)
                await request('/api/profile', { method: 'PUT', body: JSON.stringify(v) });
            setProfile(v);
            notify('Perfil guardado');
        }
        catch (err) {
            notify((err as Error).message);
        } }}><div className="avatar large">{profile.photo ? <img src={profile.photo} alt=""/> : profile.username.charAt(0).toUpperCase()}</div><label>Nombre de usuario<input name="username" required maxLength={50} defaultValue={profile.username}/></label><label>Correo electrónico<input name="email" type="email" required maxLength={50} defaultValue={profile.email}/></label><label>Fecha de nacimiento<input name="birthday" type="date" max={new Date().toISOString().slice(0, 10)} defaultValue={profile.birthday}/></label><label>URL de fotografía<input name="photo" type="url" defaultValue={profile.photo} placeholder="https://…"/></label><button className="primary">Guardar cambios <Check size={16}/></button></form><div className="settings-panel"><div><h3>Tu forma de ver la música</h3><p>Apariencia de PoliMusic</p></div><button className="secondary" onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}>{theme === 'dark' ? <Sun size={18}/> : <Moon size={18}/>} Modo {theme === 'dark' ? 'claro' : 'oscuro'}</button></div><div className="settings-panel"><div><h3>Acceso y seguridad</h3><p>{session ? 'Sesión autenticada en la API' : 'Estás explorando una demostración local.'}</p></div><button className="secondary" onClick={() => navigate('/login')}>Gestionar acceso <ArrowRight size={16}/></button>{session && <button className="secondary" onClick={async () => { await request('/api/auth/logout', { method: 'POST' }); setSession(false); setPlaylists([]); setProfile({ ...initialProfile, id: 0, username: 'Invitado', email: '' }); navigate('/login'); }}><LogOut size={16}/>Cerrar sesión</button>}</div></>;
    }
    else if (path === '/login' || path === '/register') {
        content = <Auth mode={path === '/register' ? 'register' : 'login'} onSuccess={p => { setProfile(p); setSession(true); navigate('/'); }}/>;
    }
    else if (path === '/admin') {
        content = <><PageTitle eyebrow="POLIMUSIC STUDIO" title="Detrás de la música." text="La gestión del catálogo requiere una sesión de administrador en la API."/>{!session || profile.role !== 1 ? <div className="empty"><Shield /><h3>Solo para administradores</h3><p>Inicia sesión con una cuenta autorizada para gestionar usuarios, canciones y relaciones.</p><Link className="primary" to="/login">Iniciar sesión</Link></div> : <Admin onNotify={notify}/>}</>;
    }
    else if (path === '/welcome') {
        content = <><div className="welcome"><span className="eyebrow">MUSIC, BEAUTIFULLY CURATED</span><h1>Your music.<br /><em>Your little universe.</em></h1><p>Un espacio para descubrir, escuchar y guardar las canciones que hacen tu mundo.</p><div className="actions"><Link className="primary" to="/explore">Explorar música <ArrowUpRight size={17}/></Link><Link className="secondary" to="/register">Crear una cuenta</Link></div></div><Section title="Una canción. Mil maneras de sentir." label="ENCUENTRA TU LUGAR"/>{albumCards()}</>;
    }
    else if (path === '/explore' || path === '/library') {
        content = <><PageTitle eyebrow={path === '/explore' ? 'DÉJATE SORPRENDER' : 'TU COLECCIÓN'} title={path === '/explore' ? 'Tu próxima canción favorita.' : 'La música que se queda contigo.'} text="Explora canciones, álbumes y artistas. Encuentra algo que se sienta como tú."/><div className="filterbar"><div className="tabs">{['Todo', 'Canciones', 'Álbumes', 'Artistas', 'Playlists'].map(t => <button className={filter === t ? 'chosen' : ''} key={t} onClick={() => setFilter(t)}>{t}</button>)}</div><select aria-label="Ordenar canciones" value={sort} onChange={e => setSort(e.target.value)}><option value="default">Orden original</option><option value="name">Nombre A–Z</option><option value="plays">Más escuchadas</option></select></div>{(filter === 'Todo' || filter === 'Álbumes') && <><Section title="Álbumes para explorar"/>{albumCards()}</>}{(filter === 'Todo' || filter === 'Canciones') && <><Section title="Todas las canciones"/>{rows(filtered)}</>}{(filter === 'Todo' || filter === 'Artistas') && <><Section title="Detrás de cada canción"/>{artistCards()}</>}{filter === 'Playlists' && playlistCards()}</>;
    }
    else if (path === '/') {
        content = <><div className="greeting"><div><span className="eyebrow">TU DOSIS DIARIA DE MÚSICA</span><h1>{new Date().getHours() < 12 ? 'Buenos días' : new Date().getHours() < 19 ? 'Buenas tardes' : 'Buenas noches'}<span className="lavender">.</span></h1><p>Qué bueno verte por aquí. ¿Qué escuchamos hoy?</p></div><span className="date-label">UN PEQUEÑO UNIVERSO, SOLO TUYO <span>✳</span></span></div><section className="hero"><img className="hero-photo" src="/media/hero.jpg" alt="Campo al amanecer entre montañas y niebla"/><div className="hero-shade"/><div className="hero-copy"><span className="hero-label"><span /> THE POLIMUSIC EDIT</span><h2>Your music.<br /><em>Your little universe.</em></h2><p>Hay canciones que se escuchan.<br />Y otras que se sienten como casa.</p><button className="hero-button" onClick={() => play(songs[0])}><Play size={16} fill="currentColor"/> Encuentra tu sonido</button></div><div className="hero-caption"><span>VOL. 01 — A PLACE TO BEGIN</span><span>CURATED WITH FEELING <ArrowUpRight size={16}/></span></div><span className="hero-orbit">p.</span></section><div className="mood-strip"><span>Hoy suena a…</span>{[{ text: 'Un nuevo comienzo', id: 1 }, { text: 'Bajar las revoluciones', id: 2 }, { text: 'Un poco de nostalgia', id: 3 }].map((m, i) => <button key={m.id} onClick={() => navigate(`/album/${m.id}`)}><span className={`mood-dot dot-${i}`}/>{m.text}<ArrowUpRight size={14}/></button>)}</div><Section title="Dale play a otro mundo." label="ÁLBUMES DESTACADOS" link="Ver todos los álbumes" href="/explore"/>{albumCards()}<div className="lower-grid"><section><Section title="En repeat." label="LO MÁS ESCUCHADO" link="Ver todo" href="/explore"/>{rows([...songs].sort((a, b) => b.plays - a.plays).slice(0, 3))}</section><div className="editorial-note"><span className="eyebrow">MADE OF LITTLE MOMENTS</span><h2>Algunas canciones<br />son un <em>lugar.</em></h2><p>Guarda las tuyas. Vuelve cuando quieras.</p><button onClick={newPlaylist}>Crear mi playlist <ArrowUpRight size={19}/></button></div></div><Section title="Detrás de cada canción." label="ARTISTAS POR DESCUBRIR" link="Explorar artistas" href="/explore"/>{artistCards()}</>;
    }
    else
        content = <Empty />;
    return <div className="app-shell"><aside className="sidebar"><Link to="/" className="brand"><AudioLines size={29}/><span>polimusic<span className="brand-dot">.</span></span></Link><span className="nav-label">TU UNIVERSO</span><nav><NavLink to="/" end><House size={19}/><span>Inicio</span></NavLink><NavLink to="/explore"><Compass size={19}/><span>Explorar</span></NavLink><NavLink to="/library"><Library size={19}/><span>Tu biblioteca</span></NavLink></nav><div className="sidebar-divider"/><div className="playlist-label"><span className="nav-label">TUS PLAYLISTS</span><button aria-label="Crear playlist" onClick={newPlaylist}><Plus size={17}/></button></div><div className="side-playlists">{playlists.slice(0, 6).map((p, i) => <NavLink key={p.id} to={`/playlist/${p.id}`}><img src={p.photo || `/resources/playlistimages/${i === 0 ? 'default.jpg' : `playlist${Math.min(i, 4)}.png`}`} alt=""/><span>{p.title}<small>{p.type ? 'Privada' : 'Pública'}</small></span></NavLink>)}</div><button className="side-create" onClick={newPlaylist}><Plus size={16}/> Crear playlist</button><div className="sidebar-bottom"><div className="tiny-note"><Headphones size={22}/><p>Tu mundo.<br /><span>Tu banda sonora.</span></p><span>✳</span></div><Link to="/profile"><Settings size={17}/> Configuración</Link><Link to="/welcome"><ArrowUpRight size={17}/> Acerca de PoliMusic</Link><button onClick={() => navigate('/admin')}><Shield size={17}/> PoliMusic Studio</button></div></aside><div className="main-shell"><header className="topbar"><div className="history"><button aria-label="Volver" onClick={() => navigate(-1)}><ChevronLeft size={20}/></button><button aria-label="Avanzar" onClick={() => navigate(1)}><ChevronRight size={20}/></button></div><label className="search"><Search size={17}/><input aria-label="Buscar música" value={query} onChange={e => { setQuery(e.target.value); if (path !== '/explore' && path !== '/library')
        navigate('/explore'); }} placeholder="Encuentra tu próxima canción…"/><kbd>⌘ K</kbd></label><div className="top-actions"><span className="demo-badge">{apiBase ? 'API' : 'Demo'}</span><button className="theme-btn" aria-label="Cambiar apariencia" onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}>{theme === 'dark' ? <Sun size={18}/> : <Moon size={18}/>}</button><Link to="/profile" className="profile-link"><span className="avatar">{profile.photo ? <img src={profile.photo} alt=""/> : profile.username[0].toUpperCase()}</span><span>Mi espacio</span></Link></div></header><main>{isPending ? <div className="loading"><LoaderCircle className="spin"/> Afinando tu universo…</div> : isError ? <div className="empty" role="alert"><AlertCircle /><h2>No pudimos cargar el catálogo.</h2><button className="primary" onClick={() => refetch()}>Intentar de nuevo</button></div> : <motion.div key={path} initial={{ opacity: 0, y: 8 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: .22 }}>{content}</motion.div>}<div className="page-footer"><span>polimusic.</span><small>Music, beautifully curated.</small><a className="source-download" href="/polimusic-source.zip" download>Descargar proyecto</a><small>{apiBase ? 'Conectado a tu API' : 'Modo demostración · cambios guardados en este navegador'}</small></div></main></div><Player onShuffle={() => { const choices = queue.filter(id => id !== current?.id); const s = songs.find(x => x.id === choices[Math.floor(Math.random() * choices.length)]); if (s)
        play(s); }} onStart={async () => { if (apiBase && session && current) {
        const result = await request<{
            eventId: string;
        }>(`/api/songs/${current.id}/start`, { method: 'POST' });
        receipt.current = result.eventId;
    } }} song={current} source={source} artists={current ? artistNames(current) : ''} onNext={() => step(1)} onPrevious={() => step(-1)} onQueue={() => setModal('queue')} onExpanded={() => setModal(current ? 'audio' : 'queue')} onPlayed={id => { if (apiBase && session)
        request(`/api/songs/${id}/plays`, { method: 'POST', body: JSON.stringify({ eventId: receipt.current }) }).catch(() => notify('No se pudo registrar la reproducción'));
    else
        setLocalPlays(p => ({ ...p, [id]: (p[id] || 0) + 1 })); }}/><nav className="mobile-nav"><NavLink to="/" end><House size={19}/>Inicio</NavLink><NavLink to="/explore"><Search size={19}/>Explorar</NavLink><NavLink to="/playlists"><Library size={19}/>Playlists</NavLink><NavLink to="/profile"><Settings size={19}/>Mi espacio</NavLink></nav>
 {toast && <div role="status" className="toast"><Check size={17}/>{toast}</div>}
 {modal === 'playlist' && <Modal title={edit ? 'Hazla más tuya.' : 'Un nuevo pequeño mundo.'} onClose={() => setModal('')}><form onSubmit={savePlaylist}><label>Nombre de tu playlist<input autoFocus name="title" required maxLength={50} defaultValue={edit?.title} placeholder="La banda sonora de…"/></label><label>Privacidad<select name="type" defaultValue={edit?.type ?? 1}><option value="1">Privada — solo para ti</option><option value="0">Pública</option></select></label><label>Imagen (URL opcional)<input type="url" name="photo" defaultValue={edit?.photo} placeholder="https://…"/></label><button className="primary" type="submit">{edit ? 'Guardar cambios' : 'Crear playlist'}<ArrowUpRight size={17}/></button></form></Modal>}
 {modal === 'delete' && edit && <Modal title="¿Eliminar esta playlist?" onClose={() => setModal('')}><p>Se eliminará “{edit.title}” y su selección de canciones.</p><div className="actions"><button className="secondary" onClick={() => setModal('')}>Conservar</button><button className="danger" onClick={() => deletePlaylist(edit)}>Eliminar playlist</button></div></Modal>}
 {(modal === 'add' || modal === 'song') && targetSong && <Modal title={targetSong.name} onClose={() => setModal('')}>{modal === 'song' ? <div className="menu-options"><button onClick={() => { setQueue(v => [...v, targetSong.id]); setModal(''); notify('Añadida a la cola'); }}><ListMusic />Añadir a la cola</button><button onClick={() => setModal('add')}><Plus />Añadir a playlist</button><button onClick={() => { play(targetSong); if (targetSong.id === 1 || uploads[targetSong.id])
        setModal(''); }}><Play />Reproducir canción</button></div> : <><p>Elige un pequeño mundo para esta canción.</p>{playlists.map(p => <button key={p.id} className="playlist-choice" onClick={() => addSong(p, targetSong)}><Music2 size={18}/>{p.title}<Plus size={16}/></button>)}{!playlists.length && <button className="primary" onClick={newPlaylist}>Crear una playlist</button>}</>}</Modal>}
 {modal === 'queue' && <Modal title="Lo que viene después." onClose={() => setModal('')}><p>Tu cola de reproducción</p>{queue.map((id, i) => { const s = songs.find(x => x.id === id); return s ? <div className="queue-row" key={`${id}-${i}`}><button onClick={() => { play(s); if (s.id === 1 || uploads[s.id])
        setModal(''); }}><Play size={15}/>{s.name}</button><button aria-label={`Quitar ${s.name} de cola`} onClick={() => setQueue(q => q.filter((_, j) => j !== i))}><X size={16}/></button></div> : null; })}{!queue.length && <p>La cola está vacía. Explora y agrega canciones.</p>}</Modal>}
 {modal === 'audio' && current && <Modal title="Ahora suena en tu universo." onClose={() => setModal('')}><div className="now-expanded"><Cover kind="rock" title={current.name}/><h2>{current.name}</h2><p>{artistNames(current)}</p>{!source && <p className="notice">Este MP3 no está incluido en los recursos adjuntos. Selecciona el archivo de esta canción para reproducirla.</p>}<label className="upload-label"><Upload size={18}/> {source ? 'Cambiar archivo de audio' : 'Seleccionar archivo de audio'}<input type="file" accept="audio/*" onChange={e => { const f = e.target.files?.[0]; if (f) {
        if (f.size > 100 * 1024 * 1024) {
            notify('El archivo no puede superar 100 MB');
            return;
        }
        const url = URL.createObjectURL(f);
        setUploads(v => ({ ...v, [current.id]: url }));
        setSource(url);
        notify('Audio cargado para esta sesión');
    } }}/></label><small>{source ? 'Controla la reproducción en la barra inferior.' : 'Adventure sí cuenta con su MP3 original.'}</small></div></Modal>}
 </div>;
}
function Section({ title, label, link, href }: {
    title: string;
    label?: string;
    link?: string;
    href?: string;
}) { return <div className="section-heading"><div>{label && <span className="eyebrow">{label}</span>}<h2>{title}</h2></div>{link && <Link to={href || '/explore'}>{link}<ArrowRight size={16}/></Link>}</div>; }
function PageTitle({ eyebrow, title, text, action }: {
    eyebrow: string;
    title: string;
    text: string;
    action?: ReactNode;
}) { return <div className="page-title"><div><span className="eyebrow">{eyebrow}</span><h1>{title}</h1><p>{text}</p></div>{action}</div>; }
function Empty() { return <div className="empty"><Music2 /><h1>Este rincón está por descubrir.</h1><Link to="/">Volver al inicio</Link></div>; }
