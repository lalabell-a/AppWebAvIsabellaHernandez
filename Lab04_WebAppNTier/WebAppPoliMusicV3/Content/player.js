(function () {
 function bindPlayers() {
    'use strict';
    var players = document.querySelectorAll('audio');
    for (var i = 0; i < players.length; i++) {
        if(players[i].getAttribute('data-bound')) continue; players[i].setAttribute('data-bound','true');
        players[i].addEventListener('play', function () {
            for (var j = 0; j < players.length; j++) if (players[j] !== this) players[j].pause();
            this.closest('tr').classList.add('playing');
        });
        players[i].addEventListener('pause', function () { this.closest('tr').classList.remove('playing'); });
        players[i].addEventListener('ended', function () { this.closest('tr').classList.remove('playing'); });
        players[i].addEventListener('error', function () {
            this.nextElementSibling.textContent = 'Audio no disponible. Comprueba el archivo MP3 o vuelve a iniciar sesión.';
            this.closest('tr').classList.remove('playing');
        });
    }
}
 if(window.Sys && Sys.Application) Sys.Application.add_load(bindPlayers); else bindPlayers();
}());
