import React from 'react';
import {renderToString} from 'react-dom/server';
import {MemoryRouter} from 'react-router-dom';
import {QueryClient,QueryClientProvider} from '@tanstack/react-query';
import assert from 'node:assert/strict';
import App from '../src/App';
import {songs,albums,artists} from '../src/data';
for(const [path,marker] of [['/','Your music.'],['/explore','Tu próxima canción favorita.'],['/library','La música que se queda contigo.'],['/album/1','Rock Album'],['/artist/3','Mariah Carey'],['/playlist/1','Playlist 1'],['/playlists','Cada playlist'],['/profile','Tan tú como tu música.'],['/login','Qué bueno que volviste.'],['/register','Empieza tu universo.'],['/admin','Solo para administradores'],['/welcome','Your music.']]){
 const client=new QueryClient();client.setQueryData(['catalog'],{songs,albums,artists});
 const html=renderToString(<QueryClientProvider client={client}><MemoryRouter initialEntries={[path]}><App/></MemoryRouter></QueryClientProvider>);
 assert(html.includes(marker),`Route ${path} missing content: ${marker}`);
 assert(!html.includes('undefined'),'Unexpected undefined content');
 client.clear();console.log('Rendered',path);
}
