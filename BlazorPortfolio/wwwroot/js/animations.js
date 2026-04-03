// ── Night sky starfield ───────────────────────────────────────────────────
(function () {
    const canvas = document.getElementById('night-sky');
    if (!canvas) return;

    // On mobile the canvas is hidden via CSS — skip all JS work entirely
    const isMobile = window.innerWidth < 768;
    if (isMobile) return;

    const ctx = canvas.getContext('2d');
    let stars = [];

    // Desktop only — keep full density
    const DENSITY  = 6000;
    // Desktop: 60fps
    const FRAME_MS = 0;
    let lastFrame  = 0;

    function resize() {
        canvas.width  = window.innerWidth;
        canvas.height = document.body.scrollHeight;
        initStars();
    }

    function initStars() {
        stars = [];
        const count = Math.floor((canvas.width * canvas.height) / DENSITY);
        for (let i = 0; i < count; i++) {
            stars.push({
                x:     Math.random() * canvas.width,
                y:     Math.random() * canvas.height,
                r:     Math.random() * 1.4 + 0.2,
                speed: Math.random() * 0.008 + 0.002,
                phase: Math.random() * Math.PI * 2,
                color: Math.random() > 0.85 ? `rgba(180,210,255,` : `rgba(255,255,255,`,
            });
        }
    }

    let raf;
    function draw(ts) {
        raf = requestAnimationFrame(draw);
        // Throttle frame rate on mobile
        if (FRAME_MS > 0 && ts - lastFrame < FRAME_MS) return;
        lastFrame = ts;

        ctx.clearRect(0, 0, canvas.width, canvas.height);
        for (const s of stars) {
            const a = 0.15 + 0.85 * (0.5 + 0.5 * Math.sin(ts * s.speed + s.phase));
            ctx.beginPath();
            ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
            ctx.fillStyle = s.color + a.toFixed(3) + ')';
            ctx.fill();
        }
    }

    // Pause animation when tab is hidden to save resources
    document.addEventListener('visibilitychange', () => {
        if (document.hidden) { cancelAnimationFrame(raf); }
        else { raf = requestAnimationFrame(draw); }
    });

    let resizeTimer;
    window.addEventListener('resize', () => {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(() => { cancelAnimationFrame(raf); resize(); raf = requestAnimationFrame(draw); }, 200);
    }, { passive: true });

    // Only resize canvas on significant height changes (debounced)
    let updateTimer;
    document.addEventListener('blazor:afterUpdate', () => {
        clearTimeout(updateTimer);
        updateTimer = setTimeout(() => {
            const newH = document.body.scrollHeight;
            if (Math.abs(canvas.height - newH) > 150) { cancelAnimationFrame(raf); resize(); raf = requestAnimationFrame(draw); }
        }, 300);
    });

    resize();
    raf = requestAnimationFrame(draw);
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
        { threshold: 0.05 }
    );

    function observe() {
        document.querySelectorAll('.reveal, .reveal-scale').forEach(el => {
            if (!el.classList.contains('visible')) io.observe(el);
        });
    }

    observe();
    document.addEventListener('DOMContentLoaded', observe);

    let revealTimer;
    document.addEventListener('blazor:afterUpdate', () => {
        clearTimeout(revealTimer);
        revealTimer = setTimeout(observe, 100);
    });

    setTimeout(() => {
        document.querySelectorAll('.reveal, .reveal-scale').forEach(el => el.classList.add('visible'));
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

    let barTimer;
    document.addEventListener('blazor:afterUpdate', () => {
        clearTimeout(barTimer);
        barTimer = setTimeout(observeBars, 100);
    });
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

// ── Collaboration carousel (drag + touch to scroll) ───────────────────────
(function () {
    function initCarousel() {
        const wrap = document.querySelector('.collab-carousel-wrap');
        if (!wrap || wrap.dataset.dragInit) return;
        wrap.dataset.dragInit = '1';

        // Mouse drag
        let isDown = false, startX = 0, scrollLeft = 0;
        wrap.addEventListener('mousedown', e => {
            isDown = true;
            startX = e.pageX - wrap.offsetLeft;
            scrollLeft = wrap.scrollLeft;
        });
        wrap.addEventListener('mouseleave', () => isDown = false);
        wrap.addEventListener('mouseup', () => isDown = false);
        wrap.addEventListener('mousemove', e => {
            if (!isDown) return;
            e.preventDefault();
            wrap.scrollLeft = scrollLeft - (e.pageX - wrap.offsetLeft - startX);
        });

        // Touch swipe
        let touchStartX = 0, touchScrollLeft = 0;
        wrap.addEventListener('touchstart', e => {
            touchStartX = e.touches[0].pageX;
            touchScrollLeft = wrap.scrollLeft;
        }, { passive: true });
        wrap.addEventListener('touchmove', e => {
            const dx = touchStartX - e.touches[0].pageX;
            wrap.scrollLeft = touchScrollLeft + dx;
        }, { passive: true });
    }

    document.addEventListener('DOMContentLoaded', initCarousel);
    let carouselTimer;
    document.addEventListener('blazor:afterUpdate', () => {
        clearTimeout(carouselTimer);
        carouselTimer = setTimeout(initCarousel, 100);
    });
})();

// ── Star Map ──────────────────────────────────────────────────────────────
(function () {
    let _tooltip = null;
    let _canvas  = null;
    let _systems = null;
    let _active  = null;

    window.drawStarMap = function () {
        const canvas = document.getElementById('starmap-canvas');
        if (!canvas) return;

        // Parse data attributes
        const rawSystems = canvas.dataset.systems;
        const rawActive  = canvas.dataset.active;
        if (!rawSystems) return;

        _canvas  = canvas;
        _systems = JSON.parse(rawSystems);
        _active  = rawActive;

        _tooltip = document.getElementById('starmap-tooltip');

        render();
        bindEvents();
    };

    function render() {
        const canvas = _canvas;
        const wrap   = canvas.parentElement;
        canvas.width  = wrap.offsetWidth;
        canvas.height = wrap.offsetHeight;
        const ctx = canvas.getContext('2d');
        const W = canvas.width, H = canvas.height;

        ctx.clearRect(0, 0, W, H);
        drawGrid(ctx, W, H);

        for (const sys of _systems) {
            const isActive = sys.Name === _active;
            drawSystem(ctx, sys, W, H, isActive);
        }
    }

    function drawGrid(ctx, W, H) {
        const step = Math.round(Math.min(W, H) / 8);
        ctx.save();
        ctx.strokeStyle = 'rgba(255,255,255,0.04)';
        ctx.lineWidth = 1;
        // vertical lines
        for (let x = 0; x < W; x += step) {
            ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, H); ctx.stroke();
        }
        // horizontal lines
        for (let y = 0; y < H; y += step) {
            ctx.beginPath(); ctx.moveTo(0, y); ctx.lineTo(W, y); ctx.stroke();
        }
        // concentric circles from center
        ctx.strokeStyle = 'rgba(255,255,255,0.03)';
        const cx = W / 2, cy = H / 2;
        for (let r = step; r < Math.max(W, H); r += step) {
            ctx.beginPath(); ctx.arc(cx, cy, r, 0, Math.PI * 2); ctx.stroke();
        }
        // RA/Dec style tick marks on edges
        ctx.fillStyle = 'rgba(255,255,255,0.18)';
        ctx.font = '10px monospace';
        ctx.textAlign = 'center';
        for (let x = step; x < W; x += step) {
            ctx.fillText(Math.round(x / W * 360) + '°', x, 12);
        }
        ctx.textAlign = 'right';
        for (let y = step; y < H; y += step) {
            ctx.fillText('+' + Math.round((1 - y / H) * 90) + '°', 28, y + 4);
        }
        ctx.restore();
    }

    function drawSystem(ctx, sys, W, H, isActive) {
        const alpha = isActive ? 1 : 0.22;
        const color = sys.Color;

        // Draw edges (constellation lines)
        ctx.save();
        ctx.globalAlpha = isActive ? 0.35 : 0.08;
        ctx.strokeStyle = color;
        ctx.lineWidth   = isActive ? 1 : 0.5;
        ctx.setLineDash([4, 6]);
        for (const e of sys.Edges) {
            const a = sys.Nodes[e.A], b = sys.Nodes[e.B];
            if (!a || !b) continue;
            ctx.beginPath();
            ctx.moveTo(a.X / 100 * W, a.Y / 100 * H);
            ctx.lineTo(b.X / 100 * W, b.Y / 100 * H);
            ctx.stroke();
        }
        ctx.setLineDash([]);
        ctx.restore();

        // Draw stars
        for (const node of sys.Nodes) {
            const x = node.X / 100 * W;
            const y = node.Y / 100 * H;
            const r = node.Size / 2;

            ctx.save();
            ctx.globalAlpha = alpha;

            // Outer glow
            const glow = ctx.createRadialGradient(x, y, 0, x, y, r * 3.5);
            glow.addColorStop(0, color);
            glow.addColorStop(1, 'transparent');
            ctx.fillStyle = glow;
            ctx.beginPath(); ctx.arc(x, y, r * 3.5, 0, Math.PI * 2); ctx.fill();

            // Core star
            ctx.fillStyle = '#fff';
            ctx.shadowColor = color;
            ctx.shadowBlur  = r * 4;
            ctx.beginPath(); ctx.arc(x, y, r * 0.7, 0, Math.PI * 2); ctx.fill();

            ctx.restore();

            // Label for active system — always visible
            if (isActive) {
                drawLabel(ctx, node, x, y, color, W, H);
            }
        }
    }

    function drawLabel(ctx, node, x, y, color, W, H) {
        const label = node.Name;
        ctx.save();
        ctx.font = '600 11px system-ui, sans-serif';
        const tw = ctx.measureText(label).width;
        const pad = 5, lh = 16;

        // Position label: prefer right, flip if near edge
        let lx = x + node.Size / 2 + 8;
        let ly = y - lh / 2;
        if (lx + tw + pad * 2 > W - 4) lx = x - node.Size / 2 - 8 - tw - pad * 2;
        if (ly < 4) ly = 4;
        if (ly + lh > H - 4) ly = H - lh - 4;

        // Background pill
        ctx.fillStyle = 'rgba(10,12,18,0.85)';
        ctx.strokeStyle = color;
        ctx.lineWidth = 0.8;
        ctx.globalAlpha = 0.95;
        roundRect(ctx, lx, ly, tw + pad * 2, lh, 4);
        ctx.fill(); ctx.stroke();

        // Text
        ctx.globalAlpha = 1;
        ctx.fillStyle = '#e8eaf0';
        ctx.textBaseline = 'middle';
        ctx.fillText(label, lx + pad, ly + lh / 2);
        ctx.restore();
    }

    function roundRect(ctx, x, y, w, h, r) {
        ctx.beginPath();
        ctx.moveTo(x + r, y);
        ctx.lineTo(x + w - r, y); ctx.arcTo(x + w, y, x + w, y + r, r);
        ctx.lineTo(x + w, y + h - r); ctx.arcTo(x + w, y + h, x + w - r, y + h, r);
        ctx.lineTo(x + r, y + h); ctx.arcTo(x, y + h, x, y + h - r, r);
        ctx.lineTo(x, y + r); ctx.arcTo(x, y, x + r, y, r);
        ctx.closePath();
    }

    function bindEvents() {
        const canvas = _canvas;
        if (canvas.dataset.smBound) return;
        canvas.dataset.smBound = '1';

        // Hover tooltip for inactive systems
        canvas.addEventListener('mousemove', e => {
            const rect = canvas.getBoundingClientRect();
            const mx = e.clientX - rect.left;
            const my = e.clientY - rect.top;
            const W = canvas.width, H = canvas.height;
            let hit = null;

            for (const sys of _systems) {
                if (sys.Name === _active) continue; // active already has labels
                for (const node of sys.Nodes) {
                    const nx = node.X / 100 * W;
                    const ny = node.Y / 100 * H;
                    const dist = Math.hypot(mx - nx, my - ny);
                    if (dist < node.Size + 6) { hit = { node, nx, ny }; break; }
                }
                if (hit) break;
            }

            if (hit && _tooltip) {
                _tooltip.textContent = hit.node.Name;
                _tooltip.style.display = 'block';
                _tooltip.style.left = hit.nx + 'px';
                _tooltip.style.top  = hit.ny + 'px';
            } else if (_tooltip) {
                _tooltip.style.display = 'none';
            }
        });

        canvas.addEventListener('mouseleave', () => {
            if (_tooltip) _tooltip.style.display = 'none';
        });

        // Resize
        let resizeTimer;
        window.addEventListener('resize', () => {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(() => { if (_canvas) render(); }, 200);
        }, { passive: true });
    }
})();
