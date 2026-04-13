window.alarmaAudio = {
    _audio: null,
    _sonando: false,

    inicializar: function (src) {
        this._audio = new Audio(src);
        this._audio.loop = true;
        // Desbloquea el audio en el primer click del usuario (requerido por WebView)
        document.addEventListener('click', function desbloquear() {
            if (window.alarmaAudio._audio) {
                window.alarmaAudio._audio.play()
                    .then(() => { window.alarmaAudio._audio.pause(); window.alarmaAudio._audio.currentTime = 0; })
                    .catch(() => {});
            }
            document.removeEventListener('click', desbloquear);
        }, { once: true });
    },

    sonar: function () {
        if (this._audio && !this._sonando) {
            this._audio.play().catch(e => console.warn("Audio bloqueado:", e));
            this._sonando = true;
        }
    },

    detener: function () {
        if (this._audio) {
            this._audio.pause();
            this._audio.currentTime = 0;
            this._sonando = false;
        }
    }
};