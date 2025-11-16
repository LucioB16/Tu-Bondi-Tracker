class LedBoard {
    constructor(element) {
        this.element = element;
        this.title = element.querySelector('.led-board__title');
        this.body = element.querySelector('.led-board__body');
        this.clock = element.querySelector('.led-board__clock');
        this.status = element.querySelector('.led-board__status');
        this.contrastBtn = element.querySelector('.led-board__contrast');
        this.endpoint = element.dataset.endpoint;
        this.stop = element.dataset.stop;
        this.stopDescription = element.dataset.stopDescription;
        this.lines = element.dataset.lines;
        this.refreshMs = Number(element.dataset.refreshMs || 25000);
        this.timer = null;
        this.clockTimer = null;
        this.lastUpdated = null;
        this.init();
    }

    init() {
        this.title.textContent = `${this.stopDescription}`;
        this.contrastBtn?.addEventListener('click', () => this.toggleContrast());
        this.startClock();
        this.fetchLoop();
    }

    toggleContrast() {
        const isActive = this.element.classList.toggle('led-board--contrast');
        this.contrastBtn.setAttribute('aria-pressed', String(isActive));
    }

    startClock() {
        const updateClock = () => {
            const now = new Date();
            const utcMillis = now.getTime() + (now.getTimezoneOffset() * 60000);
            const cordoba = new Date(utcMillis + (-3 * 60 * 60000));
            this.clock.textContent = cordoba.toLocaleTimeString('es-AR', { hour12: false });
            if (this.lastUpdated) {
                this.status.textContent = `Actualizado ${this.lastUpdated}`;
            } else {
                this.status.textContent = 'Sin datos aún';
            }
            this.clockTimer = requestAnimationFrame(updateClock);
        };
        this.clockTimer = requestAnimationFrame(updateClock);
    }

    async fetchLoop() {
        try {
            await this.fetchArrivals();
        } catch (err) {
            console.error('TuBondi fetch error', err);
            this.status.textContent = 'Error consultando la API';
        } finally {
            clearTimeout(this.timer);
            this.timer = setTimeout(() => this.fetchLoop(), this.refreshMs);
        }
    }

    async fetchArrivals() {
        const url = new URL(this.endpoint, window.location.origin);
        url.searchParams.set('stop', this.stop);
        url.searchParams.set('lines', this.lines);
        const response = await fetch(url, { cache: 'no-store' });
        const data = await response.json();
        const headers = response.headers;
        const headerTime = headers.get('X-Board-GeneratedAt');
        if (headerTime) {
            const parsed = new Date(headerTime);
            this.lastUpdated = parsed.toLocaleTimeString('es-AR', { hour12: false });
        }
        const notificationHeader = headers.get('X-Board-Notifications');
        const notifications = notificationHeader ? notificationHeader.split('|').filter(Boolean) : [];
        this.render(data, notifications);
    }

    render(arrivals, notifications) {
        this.body.innerHTML = '';
        if (notifications.length > 0) {
            notifications.forEach(message => {
                const alertRow = document.createElement('div');
                alertRow.className = 'led-row led-row--alert';
                alertRow.textContent = `ALERTA · ${message}`;
                this.body.appendChild(alertRow);
            });
        }

        if (!arrivals || arrivals.length === 0) {
            const empty = document.createElement('div');
            empty.className = 'led-row led-row__alert';
            empty.textContent = 'Sin arribos en esta ventana';
            this.body.appendChild(empty);
            return;
        }

        arrivals.forEach(arrival => {
            const row = document.createElement('div');
            row.className = 'led-row';
            row.style.setProperty('--row-color', arrival.color || '#ffb300');

            const line = document.createElement('div');
            line.className = 'led-row__line';
            line.textContent = arrival.line;

            const route = document.createElement('div');
            route.className = 'led-row__route';
            const direction = arrival.direction ? ` · ${arrival.direction}` : '';
            route.textContent = `${arrival.route}${direction}`;

            const eta = document.createElement('div');
            eta.className = 'led-row__eta';
            const minutes = arrival.etaMinutes !== null && arrival.etaMinutes !== undefined
                ? `${arrival.etaMinutes} min`
                : '---';
            const distance = arrival.distanceKm ? `${arrival.distanceKm.toFixed(1)} km` : '';
            eta.innerHTML = `${minutes}<span>${distance}</span>`;

            row.appendChild(line);
            row.appendChild(route);
            row.appendChild(eta);

            this.body.appendChild(row);
        });
    }
}

window.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.led-board').forEach(element => new LedBoard(element));
});
