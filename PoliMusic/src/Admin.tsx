import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Trash2 } from 'lucide-react';
import { request } from './data';
import Modal from './Modal';
export default function Admin({ onNotify }: {
    onNotify: (s: string) => void;
}) {
    const [kind, setKind] = useState('songs'), [search, setSearch] = useState(''), [page, setPage] = useState(0), [editing, setEditing] = useState<Record<string, unknown> | null>(null), [deleting, setDeleting] = useState<Record<string, unknown> | null>(null);
    const { data, refetch, isPending, isError } = useQuery({ queryKey: ['admin', kind], queryFn: () => request<Record<string, unknown>[]>(`/api/admin/${kind}`) });
    const { data: stats } = useQuery({ queryKey: ['stats'], queryFn: () => request<Record<string, number>>('/api/admin/stats') });
    const labels: Record<string, string> = { songs: 'Canciones', artists: 'Artistas', albums: 'Álbumes', users: 'Usuarios', tracklists: 'Canciones por álbum', players: 'Canciones por artista' };
    const fields: Record<string, [
        string,
        string,
        string
    ][]> = { songs: [['name', 'Nombre', 'text'], ['path', 'Ruta del archivo de audio', 'text']], artists: [['name', 'Nombre', 'text'], ['lastname', 'Apellido', 'text'], ['image', 'Imagen', 'text']], albums: [['title', 'Título', 'text'], ['year', 'Año', 'number'], ['image', 'Imagen', 'text']], users: [['id', 'ID de usuario', 'number'], ['role', 'Tipo: 0 usuario · 1 administrador', 'number']], tracklists: [['albumId', 'ID de álbum', 'number'], ['songId', 'ID de canción', 'number']], players: [['artistId', 'ID de artista', 'number'], ['songId', 'ID de canción', 'number']] };
    const items = (data || []).filter(x => JSON.stringify(x).toLowerCase().includes(search.toLowerCase()));
    const columns = Object.keys(items[0] || {});
    return <>{stats && <div className="admin-stats">{Object.entries(stats).map(([k, v]) => <div key={k}><strong>{v}</strong><small>{labels[k] || 'Reproducciones'}</small></div>)}</div>}<div className="tabs">{Object.keys(labels).map(k => <button key={k} onClick={() => { setKind(k); setEditing(null); setPage(0); setSearch(''); }} className={kind === k ? 'chosen' : ''}>{labels[k]}</button>)}</div><div className="admin-tools"><input aria-label="Buscar registros" placeholder="Buscar registros…" value={search} onChange={e => { setSearch(e.target.value); setPage(0); }}/><button className="primary" onClick={() => setEditing({})}>{kind === 'users' ? 'Cambiar rol' : kind === 'players' || kind === 'tracklists' ? 'Asociar' : 'Crear registro'}</button></div>{isPending ? <p>Cargando…</p> : isError ? <p role="alert">No se pudo obtener la información.</p> : !items.length ? <p>No se encontraron registros.</p> : <div className="admin-table"><table><thead><tr>{columns.map(k => <th key={k}>{k}</th>)}<th>Acciones</th></tr></thead><tbody>{items.slice(page * 10, (page + 1) * 10).map((x, i) => <tr key={i}>{columns.map(k => <td key={k}>{String(x[k] ?? '—')}</td>)}<td><button onClick={() => setEditing(x)}>Editar</button>{kind !== 'users' && <button aria-label="Eliminar registro" onClick={() => setDeleting(x)}><Trash2 size={15}/></button>}</td></tr>)}</tbody></table></div>}{items.length > 10 && <div className="actions"><button disabled={page === 0} onClick={() => setPage(p => p - 1)}>Anterior</button><small>Página {page + 1} de {Math.ceil(items.length / 10)}</small><button disabled={(page + 1) * 10 >= items.length} onClick={() => setPage(p => p + 1)}>Siguiente</button></div>}
 {editing && <Modal title={editing.id ? 'Editar registro' : 'Nuevo registro'} onClose={() => setEditing(null)}><form onSubmit={async (e) => { e.preventDefault(); const f = new FormData(e.currentTarget); const value: Record<string, unknown> = { ...editing }; for (const [key, , type] of fields[kind]) {
        const raw = String(f.get(key) || '');
        value[key] = type === 'number' ? (raw ? +raw : null) : raw;
    } try {
        await request(`/api/admin/${kind}`, { method: 'POST', body: JSON.stringify(value) });
        setEditing(null);
        refetch();
        onNotify('Cambios guardados');
    }
    catch (err) {
        onNotify((err as Error).message);
    } }}>{fields[kind].map(([key, label, type]) => <label key={key}>{label}<input name={key} type={type} required={!['lastname', 'image', 'year'].includes(key)} defaultValue={String(editing[key] ?? (key === 'image' ? editing.cover ?? '' : ''))} maxLength={type === 'text' ? key === 'name' && kind === 'songs' ? 50 : 255 : undefined}/></label>)}<button className="primary">Guardar</button></form></Modal>}
 {deleting && <Modal title="¿Eliminar este registro?" onClose={() => setDeleting(null)}><p>Las relaciones existentes deben quitarse primero.</p><button className="danger" onClick={async () => { const id = deleting.id || deleting.albumId || deleting.artistId; try {
        await request(`/api/admin/${kind}/${id}${deleting.songId ? `?songId=${deleting.songId}` : ''}`, { method: 'DELETE' });
        setDeleting(null);
        refetch();
        onNotify('Registro eliminado');
    }
    catch (err) {
        onNotify((err as Error).message);
    } }}>Eliminar registro</button></Modal>}</>;
}
