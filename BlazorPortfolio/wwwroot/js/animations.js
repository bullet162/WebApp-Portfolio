// ── Night sky starfield ───────────────────────────────────────────────────
(function () {
    const canvas = document.getElementById('night-sky');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    let stars = [];

    function resize() {
        canvas.width  = window.innerWidth;
        canvas.height = document.body.scrollHeight;
        initStars();
    }

    function initStars() {
        stars = [];
        const count = Math.floor((canvas.width * canvas.height) / 6000);
        for (let i = 0; i < count; i++) {
            stars.push({
                x:       Math.random() * canvas.width,
                y:       Math.random() * canvas.height,
                r:       Math.random() * 1.4 + 0.2,
                alpha:   Math.random(),
                speed:   Math.random() * 0.008 + 0.002,
                phase:   Math.random() * Math.PI * 2,
                color:   Math.random() > 0.85
                    ? `rgba(180,210,255,`   // slightly blue-white
                    : `rgba(255,255,255,`,  // pure white
            });
        }
    }

    let raf;
    function draw(ts) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        for (const s of stars) {
            const a = 0.15 + 0.85 * (0.5 + 0.5 * Math.sin(ts * s.speed + s.phase));
            ctx.beginPath();
            ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
            ctx.fillStyle = s.color + a.toFixed(3) + ')';
            ctx.fill();
        }
        raf = requestAnimationFrame(draw);
    }

    resize();
    window.addEventListener('resize', () => { cancelAnimationFrame(raf); resize(); draw(0); }, { passive: true });
    // Resize when page content changes height (Blazor renders)
    document.addEventListener('blazor:afterUpdate', () => {
        const newH = document.body.scrollHeight;
        if (Math.abs(canvas.height - newH) > 100) { cancelAnimationFrame(raf); resize(); draw(0); }
    });
    requestAnimationFrame(draw);
})();
(function () {
    const io = new IntersectionObserver(
        (entries) => {
            entries.forEach(e => {
                if (e.isIntersecting) {
                    e.target.classList.add('visible');
                    io.unobserve(e.target);
                }
            });
        },
        { threshold: 0.05, rootMargin: '0px 0px 0px 0px' }
    );

    function observe() {
        document.querySelectorAll('.reveal, .reveal-scale').forEach(el => {
            if (!el.classList.contains('visible')) io.observe(el);
        });
    }

    // Run immediately, after DOM ready, and after every Blazor re-render
    observe();
    document.addEventListener('DOMContentLoaded', observe);
    document.addEventListener('blazor:afterUpdate', () => {
        // Small delay so Blazor has finished patching the DOM
        setTimeout(observe, 50);
    });

    // Nuclear fallback: if anything is still hidden after 3s, force-show it
    setTimeout(() => {
        document.querySelectorAll('.reveal, .reveal-scale').forEach(el => {
            el.classList.add('visible');
        });
    }, 3000);
})();

// ── Skill bar animation ───────────────────────────────────────────────────
(function () {
    const barIo = new IntersectionObserver(
        (entries) => {
            entries.forEach(e => {
                if (e.isIntersecting) {
                    const fill = e.target;
                    const target = fill.dataset.pct || '0';
                    setTimeout(() => { fill.style.width = target + '%'; }, 80);
                    barIo.unobserve(fill);
                }
            });
        },
        { threshold: 0.1 }
    );

    function observeBars() {
        document.querySelectorAll('.skill-fill[data-pct]').forEach(el => barIo.observe(el));
    }

    document.addEventListener('DOMContentLoaded', observeBars);
    document.addEventListener('blazor:afterUpdate', () => setTimeout(observeBars, 50));
})();

// ── Sticky header shadow on scroll ───────────────────────────────────────
(function () {
    function init() {
        const header = document.querySelector('.site-header');
        if (!header) return;
        window.addEventListener('scroll', () => {
            header.classList.toggle('scrolled', window.scrollY > 20);
        }, { passive: true });
    }
    document.addEventListener('DOMContentLoaded', init);
})();

// ── Tech Constellation canvas lines ──────────────────────────────────────
function hexToRgb(hex) {
    const r = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return r ? `${parseInt(r[1],16)},${parseInt(r[2],16)},${parseInt(r[3],16)}` : '0,229,255';
}

window.drawConstellation = function () {
    const canvas = document.getElementById('constellation-bg');
    if (!canvas) return;
    const wrap = canvas.parentElement;
    canvas.width  = wrap.offsetWidth;
    canvas.height = wrap.offsetHeight;
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const allNodes = Array.from(document.querySelectorAll('.constellation-node'));
    if (allNodes.length < 2) return;

    const wr = wrap.getBoundingClientRect();
    const ptMap = {};
    allNodes.forEach(n => {
        if (n.classList.contains('cn-hidden')) return;
        const idx = parseInt(n.dataset.index, 10);
        const dot = n.querySelector('.cn-dot');
        const r   = (dot || n).getBoundingClientRect();
        ptMap[idx] = { x: r.left + r.width / 2 - wr.left, y: r.top + r.height / 2 - wr.top };
    });

    let edges = null;
    try { edges = JSON.parse(canvas.dataset.edges); } catch(e) {}

    const color = canvas.dataset.color || '#00e5ff';
    const rgb   = hexToRgb(color);

    if (edges && edges.length > 0) {
        edges.forEach(({ a, b, weight }) => {
            const pa = ptMap[a], pb = ptMap[b];
            if (!pa || !pb) return;
            const alpha = 0.12 + weight * 0.55;
            const lw    = 0.6 + weight * 1.8;
            ctx.shadowColor = `rgba(${rgb},0.4)`;
            ctx.shadowBlur  = 5;
            ctx.beginPath();
            ctx.moveTo(pa.x, pa.y);
            ctx.lineTo(pb.x, pb.y);
            ctx.strokeStyle = `rgba(${rgb},${alpha})`;
            ctx.lineWidth = lw;
            ctx.stroke();
        });
        ctx.shadowBlur = 0;
    }
};

document.addEventListener('blazor:afterUpdate', () => {
    requestAnimationFrame(window.drawConstellation);
});
window.addEventListener('resize', window.drawConstellation, { passive: true });
