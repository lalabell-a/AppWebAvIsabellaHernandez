import { useRef, useEffect, useState } from 'react';
import { Play, Pause, SkipBack, SkipForward, Shuffle, Repeat, Volume2, VolumeX, ListMusic, Maximize2, ChevronDown, Music2 } from 'lucide-react';
import type { Song } from './data';
export default function Player({ song, source, onNext, onPrevious, onQueue, artists, onExpanded, onPlayed, onStart, onShuffle }: {
    onStart: () => Promise<void>;
    onShuffle: () => void;
    song: Song | null;
    source: string;
    onNext: () => void;
    onPrevious: () => void;
    onQueue: () => void;
    artists: string;
    onExpanded: () => void;
    onPlayed: (id: number) => void;
}) {
    const audio = useRef<HTMLAudioElement>(null);
    const [playing, setPlaying] = useState(false);
    const [time, setTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [volume, setVolume] = useState(.65);
    const [shuffle, setShuffle] = useState(false);
    const [repeat, setRepeat] = useState(false);
    const counted = useRef(false);
    const [error, setError] = useState('');
    useEffect(() => { counted.current = false; setTime(0); setError(''); if (source)
        onStart().catch(() => { }); if (source)
        audio.current?.play().catch(() => setPlaying(false)); }, [source, song?.id]);
    useEffect(() => { if (audio.current)
        audio.current.volume = volume; }, [volume]);
    function toggle() { if (!source)
        return onExpanded(); const a = audio.current!; if (a.paused)
        a.play().catch(() => setError('No se pudo reproducir este archivo.'));
    else
        a.pause(); }
    useEffect(() => { function key(e: KeyboardEvent) { if (e.code === 'Space' && !(e.target as HTMLElement).matches('input,textarea,button,select')) {
        e.preventDefault();
        toggle();
    } } window.addEventListener('keydown', key); return () => window.removeEventListener('keydown', key); }, [source]);
    function fmt(t: number) { return `${Math.floor(t / 60)}:${String(Math.floor(t % 60)).padStart(2, '0')}`; }
    const next = () => { if (shuffle)
        onShuffle();
    else
        onNext(); };
    return <footer className="player"><audio ref={audio} src={source || undefined} onPlay={() => setPlaying(true)} onPause={() => setPlaying(false)} onLoadedMetadata={e => setDuration(e.currentTarget.duration)} onTimeUpdate={e => { const a = e.currentTarget; setTime(a.currentTime); if (!counted.current && song && a.currentTime >= 30) {
        counted.current = true;
        onPlayed(song.id);
    } }} onEnded={() => { if (repeat) {
        audio.current!.currentTime = 0;
        audio.current!.play();
    }
    else
        next(); }} onError={() => { setError('Archivo no disponible. Carga un audio desde la vista ampliada.'); setPlaying(false); }}/>
 <button className="now-song" onClick={onExpanded} aria-label="Abrir canción actual"><img src="/media/rock.jpg" alt=""/><span><strong>{song?.name || 'Tu próxima canción'}</strong><small>{song ? artists : 'Un universo por descubrir'}</small>{error && <small role="alert">{error}</small>}</span></button>
 <div className="play-center"><div className="transport"><button aria-label="Aleatorio" aria-pressed={shuffle} className={shuffle ? 'active' : ''} onClick={() => setShuffle(!shuffle)}><Shuffle size={16}/></button><button aria-label="Anterior" onClick={onPrevious}><SkipBack size={19}/></button><button className="play-button" aria-label={playing ? 'Pausar' : 'Reproducir'} onClick={toggle}>{playing ? <Pause size={18} fill="currentColor"/> : <Play size={18} fill="currentColor"/>}</button><button aria-label="Siguiente" onClick={next}><SkipForward size={19}/></button><button aria-label="Repetir canción" aria-pressed={repeat} className={repeat ? 'active' : ''} onClick={() => setRepeat(!repeat)}><Repeat size={16}/></button></div><div className="timeline"><span>{fmt(time)}</span><input aria-label="Progreso de reproducción" type="range" min="0" max={duration || 1} step="0.1" value={time} onChange={e => { if (audio.current)
        audio.current.currentTime = +e.target.value; setTime(+e.target.value); }}/><span>{fmt(duration)}</span></div></div>
 <div className="sound-controls"><button onClick={onQueue} aria-label="Cola de reproducción"><ListMusic size={18}/></button><button onClick={() => setVolume(volume ? 0 : .65)} aria-label={volume ? 'Silenciar' : 'Activar sonido'}>{volume ? <Volume2 size={18}/> : <VolumeX size={18}/>}</button><input aria-label="Volumen" type="range" min="0" max="1" step=".01" value={volume} onChange={e => setVolume(+e.target.value)}/><button onClick={onExpanded} aria-label="Ampliar reproductor"><Maximize2 size={17}/></button></div></footer>;
}
