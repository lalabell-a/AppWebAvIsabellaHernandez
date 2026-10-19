export type Song = {
    id: number;
    name: string;
    path: string;
    plays: number;
    artistIds: number[];
};
export type Album = {
    id: number;
    title: string;
    year: number;
    songIds: number[];
    cover: string;
};
export type Artist = {
    id: number;
    name: string;
    image: string;
};
export type Playlist = {
    id: number;
    userId: number;
    title: string;
    type: number;
    photo: string;
    created: string;
    songIds: number[];
};
export type Profile = {
    id: number;
    username: string;
    email: string;
    birthday: string;
    photo: string;
    role: number;
};
export const songs: Song[] = ['Adventure', 'Dreams', 'Energy', 'Scifi', 'Tomorrow'].map((name, i) => ({ id: i + 1, name, path: `~/songFiles/bensound-${name.toLowerCase()}.mp3`, plays: i, artistIds: [1, i < 2 ? 2 : i + 1] }));
export const albums: Album[] = [['Rock Album', 1976, [1, 2, 3, 4, 5], 'rock'], ['Relax Music', 2000, [2, 3], 'relax'], ['Retro Album Music', 1980, [1, 3, 5], 'retro'], ['Classic Album', 1950, [2, 4], 'classic'], ['Country Album', 1990, [3, 5], 'country']].map((a, i) => ({ id: i + 1, title: a[0] as string, year: a[1] as number, songIds: a[2] as number[], cover: a[3] as string }));
export const artists: Artist[] = ['Juan Gabriel', 'Ana Gabriel', 'Mariah Carey', 'Elvis Presley', 'Edward Sheeran'].map((name, i) => ({ id: i + 1, name, image: ['1464822759023-fed622ff2c3b', '1470252649378-9c29740c9fa8', '1519681393784-d120267933ba', '1500530855697-b586d89ba3ee', '1470770841072-f978cf4d019e'][i] }));
// Artist imagery is intentionally abstract: source photographs were not provided.
export const initialPlaylists: Playlist[] = [[1, 2, 3, 4, 5], [1, 2, 3, 4], [4], [1, 3], [5]].map((songIds, i) => ({ id: i + 1, userId: 1, title: `Playlist ${i + 1}`, type: [1, 0, 0, 1, 0][i], photo: `/resources/playlistimages/${i === 0 ? 'default.jpg' : `playlist${i}.png`}`, created: `2026-12-${12 + i}`, songIds }));
export const initialProfile: Profile = { id: 1, username: 'userNormal', email: 'usrnormal@epn.edu.ec', birthday: '2000-12-15', photo: '', role: 0 };
export function readLocal<T>(key: string, fallback: T): T { try {
    return JSON.parse(localStorage.getItem(key) || 'null') ?? fallback;
}
catch {
    return fallback;
} }
export const apiBase = (import.meta as unknown as {
    env: Record<string, string>;
}).env.VITE_API_URL || '';
export async function request<T>(path: string, options: RequestInit = {}): Promise<T> { const r = await fetch(`${apiBase}${path}`, { credentials: 'include', ...options, headers: { 'Content-Type': 'application/json', ...options.headers } }); if (!r.ok) {
    const e = await r.json().catch(() => ({ message: 'No se pudo completar la solicitud' }));
    throw new Error(e.message || 'No se pudo completar la solicitud');
} return r.status === 204 ? undefined as T : r.json(); }
export const catalogService = { async load() { if (apiBase)
        return request<{
            songs: Song[];
            albums: Album[];
            artists: Artist[];
        }>('/api/catalog'); return { songs, albums, artists }; }, async auth(mode: string, body: unknown) { if (!apiBase)
        throw new Error('El acceso seguro requiere conectar la API. Puedes explorar la demostración sin una cuenta.'); return request<Profile>(`/api/auth/${mode}`, { method: 'POST', body: JSON.stringify(body) }); } };
