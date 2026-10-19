/* ==========================================================================
   Shared layout + behaviour for every page.
   Pages only need:  <div id="site-header"></div> ... <div id="site-footer"></div>
   Project pages also get a previous/next pager from <div id="project-pager"></div>.
   ========================================================================== */

const SITE = {
  name: "Fredrik Hanses",
  title: "Game Programmer",
  location: "Stockholm, Sweden",
  email: "fredrik.hanses@gmail.com",
  linkedin: "https://www.linkedin.com/in/fredrikhanses/",
  github: "https://github.com/fredrikhanses",
  cv: "assets/cv.pdf",
};

// Project pages in the order the front page lists them; the pager follows this order.
const PROJECTS = [
  { slug: "battlemarked",  name: "Battlemarked" },
  { slug: "demeo",         name: "Demeo" },
  { slug: "demeobattles",  name: "Demeo Battles" },
  { slug: "grimkeep",      name: "GrimKeep" },
  { slug: "marenostrum",   name: "Mare Nostrum" },
  { slug: "bullethell",    name: "Bullet Hell" },
  { slug: "alter",         name: "Alter" },
  { slug: "projectbrawl",  name: "Project Brawl" },
  { slug: "arrowhood",     name: "Arrowhood" },
  { slug: "seventhcircle", name: "Seventh Circle" },
  { slug: "sumodash",      name: "Sumo Dash" },
];

const ICONS = {
  arrowLeft: '<svg aria-hidden="true" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M19 12H5M12 19l-7-7 7-7"/></svg>',
  soundOn: '<svg viewBox="0 0 24 24"><path d="M3 9v6h4l5 5V4L7 9zm13.5 3A4.5 4.5 0 0 0 14 8v8a4.5 4.5 0 0 0 2.5-4zM14 3.2v2.1a7 7 0 0 1 0 13.4v2.1a9 9 0 0 0 0-17.6z"/></svg>',
  soundOff: '<svg viewBox="0 0 24 24"><path d="M16.5 12A4.5 4.5 0 0 0 14 8v2.2l2.45 2.45c.03-.2.05-.42.05-.65zM19 12a7 7 0 0 1-.55 2.7l1.5 1.5A8.9 8.9 0 0 0 21 12a9 9 0 0 0-7-8.8v2.1A7 7 0 0 1 19 12zM4.3 3 3 4.3 7.7 9H3v6h4l5 5v-6.7l4.25 4.25A7 7 0 0 1 14 18.7v2.1a9 9 0 0 0 3.7-1.8l2 2L21 19.7l-9-9zM12 4 9.9 6.1 12 8.2z"/></svg>',
};

const currentPage = location.pathname.split("/").pop().replace(".html", "") || "index";
const isHome = currentPage === "index";

/* ---------- Header / footer / pager ---------- */

function brand(href) {
  return `<a class="brand" href="${href}"><img class="brand-avatar" src="assets/images/avatar.jpg" alt=""><span class="brand-name">${SITE.name}</span></a>`;
}

function renderHeader() {
  const el = document.getElementById("site-header");
  if (!el) return;
  el.outerHTML = isHome ? `
  <header class="site-header">
    <div class="wrap">
      ${brand("#top")}
      <nav class="main-nav" aria-label="Primary">
        <a href="#work">Work</a>
        <a href="#education">Education</a>
        <a href="#about">About</a>
        <a href="#contact">Contact</a>
        <a class="nav-cv" href="${SITE.cv}" target="_blank" rel="noopener">Download CV</a>
      </nav>
    </div>
  </header>` : `
  <header class="site-header">
    <div class="wrap">
      <a class="back-link" href="index.html#work">${ICONS.arrowLeft}All work</a>
      ${brand("index.html")}
    </div>
  </header>`;
}

function renderFooter() {
  const el = document.getElementById("site-footer");
  if (!el) return;
  const bar = `
      <div class="footer-bar">
        <span>${SITE.name} · ${SITE.title} · ${SITE.location}</span>
        <nav aria-label="Contact">
          <a href="mailto:${SITE.email}">${SITE.email}</a>
          <a href="${SITE.github}" target="_blank" rel="noopener">GitHub</a>
          <a href="${SITE.linkedin}" target="_blank" rel="noopener">LinkedIn</a>
        </nav>
      </div>`;
  el.outerHTML = isHome ? `
  <footer class="site-footer" id="contact">
    <div class="wrap">
      <div class="footer-cta">
        <h2>Let's make something <span class="accent">playable.</span></h2>
        <div class="btn-group">
          <a class="btn btn-solid btn-lg" href="mailto:${SITE.email}">Email me</a>
          <a class="btn btn-lg" href="${SITE.cv}" target="_blank" rel="noopener">Download CV</a>
        </div>
      </div>${bar}
    </div>
  </footer>
  <button class="back-to-top" aria-label="Back to top">&uarr;</button>` : `
  <footer class="site-footer compact">
    <div class="wrap">${bar}
    </div>
  </footer>
  <button class="back-to-top" aria-label="Back to top">&uarr;</button>`;
}

function renderPager() {
  const el = document.getElementById("project-pager");
  if (!el) return;
  const i = PROJECTS.findIndex(p => p.slug === currentPage);
  const prev = PROJECTS[i - 1];
  const next = PROJECTS[i + 1];
  const link = (p, cls, label, text) => `
        <a class="${cls}" href="${p ? `${p.slug}.html` : "index.html#work"}">
          <small>${label}</small><strong>${text}</strong>
        </a>`;
  el.outerHTML = `
  <nav class="pager wrap" aria-label="More projects">
    <div class="pager-grid">${link(prev, "prev", prev ? "Previous project" : "Previous", `← ${prev ? prev.name : "All work"}`)}${link(next, "next", next ? "Next project" : "Next", `${next ? next.name : "All work"} →`)}
    </div>
  </nav>`;
}

/* ---------- Missing asset placeholders ---------- */

function markMissing(el, src = el.getAttribute("src") || el.dataset.src || "") {
  el.classList.add("missing-asset");
  el.dataset.missing = `Missing: ${src}`;
  if (el.tagName === "IMG") el.removeAttribute("src");
}

function initAssetFallbacks() {
  document.querySelectorAll("img").forEach(img => {
    if (img.complete && img.naturalWidth === 0 && img.getAttribute("src")) markMissing(img);
    else img.addEventListener("error", () => markMissing(img), { once: true });
  });
}

/* ---------- Videos ---------- */

function initVideos() {
  // Feature videos: autoplay muted while on screen; clips with sound also get a sound toggle.
  const autoVideos = document.querySelectorAll("video[data-autoplay]");
  const io = new IntersectionObserver(entries => {
    entries.forEach(({ target, isIntersecting }) => {
      if (isIntersecting) {
        if (!target.src && target.dataset.src) target.src = target.dataset.src;
        target.play().catch(() => {});
      } else {
        target.pause();
      }
    });
  }, { threshold: 0.25 });

  autoVideos.forEach(video => {
    video.muted = true;
    video.loop = true;
    video.playsInline = true;
    video.addEventListener("error", () => markMissing(video.parentElement, video.dataset.src), { once: true });
    io.observe(video);

    // Most clips are silent; only those marked <video data-sound> get a sound button.
    if (!video.hasAttribute("data-sound")) return;
    const btn = document.createElement("button");
    btn.className = "mute-toggle";
    btn.setAttribute("aria-label", "Toggle sound");
    btn.innerHTML = ICONS.soundOff;
    btn.addEventListener("click", () => {
      video.muted = !video.muted;
      btn.innerHTML = video.muted ? ICONS.soundOff : ICONS.soundOn;
    });
    video.parentElement.appendChild(btn);
  });

  // Hover previews (project cards / rows): load on first hover, play while hovered.
  document.querySelectorAll("[data-hover-video]").forEach(host => {
    const video = host.querySelector("video");
    if (!video) return;
    video.muted = true;
    video.loop = true;
    video.playsInline = true;
    video.addEventListener("canplay", () => video.classList.add("ready"));
    video.addEventListener("error", () => video.remove(), { once: true });
    const start = () => {
      if (!video.src && video.dataset.src) video.src = video.dataset.src;
      video.play().catch(() => {});
    };
    const stop = () => { video.pause(); };
    host.addEventListener("mouseenter", start);
    host.addEventListener("focus", start);
    host.addEventListener("mouseleave", stop);
    host.addEventListener("blur", stop);
  });
}

/* ---------- YouTube embeds ---------- */

// <div class="youtube" data-youtube="VIDEO_ID" data-title="..."></div>
// Shows the thumbnail and only loads the player on click. YouTube refuses to play
// embeds on file:// pages, so when opened locally the click opens YouTube instead.
function initYouTube() {
  document.querySelectorAll("[data-youtube]").forEach(el => {
    const id = el.dataset.youtube;
    const title = el.dataset.title || "YouTube video";
    if (!id) return markMissing(el, "YouTube video ID (data-youtube)");
    const watchUrl = `https://www.youtube.com/watch?v=${id}`;
    el.innerHTML = `
      <img src="https://i.ytimg.com/vi/${id}/maxresdefault.jpg" alt="${title}" loading="lazy">
      <button class="youtube-play" aria-label="Play ${title}"></button>`;
    // Not every video has a 1280x720 thumbnail; YouTube then answers with a 120x90
    // placeholder, so step down to 640x480 and finally the 480x360 one that always exists.
    const thumb = el.querySelector("img");
    const fallbacks = ["sddefault", "hqdefault"];
    const nextThumb = () => { if (fallbacks.length) thumb.src = `https://i.ytimg.com/vi/${id}/${fallbacks.shift()}.jpg`; };
    thumb.addEventListener("error", nextThumb);
    thumb.addEventListener("load", () => { if (thumb.naturalWidth <= 120) nextThumb(); });
    el.querySelector("button").addEventListener("click", () => {
      if (location.protocol === "file:") {
        window.open(watchUrl, "_blank", "noopener");
        return;
      }
      el.innerHTML = `<iframe src="https://www.youtube-nocookie.com/embed/${id}?autoplay=1&rel=0"
        title="${title}" allow="autoplay; encrypted-media; picture-in-picture; fullscreen"
        referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>`;
    });
  });
}

/* ---------- Modal (code popups + image lightbox) ---------- */

let modal;

function buildModal() {
  modal = document.createElement("div");
  modal.className = "modal";
  modal.setAttribute("role", "dialog");
  modal.setAttribute("aria-modal", "true");
  modal.innerHTML = `
    <div class="modal-dialog">
      <div class="modal-header">
        <h2></h2>
        <button class="modal-copy" hidden>Copy</button>
        <button class="modal-close" aria-label="Close">&times; Close</button>
      </div>
      <div class="modal-body"></div>
    </div>`;
  document.body.appendChild(modal);

  modal.addEventListener("click", e => { if (e.target === modal) closeModal(); });
  modal.querySelector(".modal-close").addEventListener("click", closeModal);
  modal.querySelector(".modal-copy").addEventListener("click", e => {
    const code = modal.querySelector("pre");
    if (!code) return;
    const done = label => {
      e.target.textContent = label;
      setTimeout(() => (e.target.textContent = "Copy"), 1500);
    };
    copyText(code.textContent).then(() => done("Copied!"), () => done("Copy failed"));
  });
  document.addEventListener("keydown", e => { if (e.key === "Escape") closeModal(); });
}

// The Clipboard API only exists on https:// and localhost, so pages opened over the
// local network (http://192.168...) fall back to the older select-and-copy approach.
function copyText(text) {
  if (navigator.clipboard && window.isSecureContext) {
    return navigator.clipboard.writeText(text).catch(() => legacyCopy(text));
  }
  return legacyCopy(text);
}

function legacyCopy(text) {
  const area = document.createElement("textarea");
  area.value = text;
  area.setAttribute("readonly", "");
  area.style.cssText = "position:fixed;top:0;left:0;opacity:0;font-size:16px";
  modal.appendChild(area);
  area.select();
  area.setSelectionRange(0, text.length);
  const ok = document.execCommand("copy");
  area.remove();
  return ok ? Promise.resolve() : Promise.reject(new Error("copy failed"));
}

function openModal({ title, content, kind = "code" }) {
  if (!modal) buildModal();
  modal.className = `modal open ${kind}`;
  modal.querySelector("h2").textContent = title;
  const body = modal.querySelector(".modal-body");
  body.replaceChildren(content);
  modal.querySelector(".modal-copy").hidden = !body.querySelector("pre");
  document.body.classList.add("modal-open");
  modal.querySelector(".modal-close").focus();
}

function closeModal() {
  if (!modal || !modal.classList.contains("open")) return;
  modal.classList.remove("open");
  document.body.classList.remove("modal-open");
}

// Syntax highlighting (highlight.js) is only downloaded the first time a code popup opens.
const HLJS_URL = "https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/highlight.min.js";
let hljsLoading;

function loadHighlighter() {
  hljsLoading ??= new Promise((resolve, reject) => {
    const s = document.createElement("script");
    s.src = HLJS_URL;
    s.onload = () => resolve(window.hljs);
    s.onerror = reject;
    document.head.appendChild(s);
  });
  return hljsLoading;
}

function highlightCode(root) {
  root.querySelectorAll("pre code").forEach(code => {
    // Drop blank lines left by pasting the code on a new line after <code>.
    code.textContent = code.textContent.replace(/^\s*\n/, "").replace(/\s+$/, "");
    if (!/\blanguage-/.test(code.className)) return;
    loadHighlighter().then(hljs => hljs.highlightElement(code)).catch(() => {});
  });
}

const CODE_LANGUAGES = { cpp: "cpp", h: "cpp", hpp: "cpp", c: "cpp", cs: "csharp" };

function codeBlock(text, language) {
  const pre = document.createElement("pre");
  const code = pre.appendChild(document.createElement("code"));
  if (language) code.className = `language-${language}`;
  code.textContent = text;
  return pre;
}

function initCodePopups() {
  // <button data-code-src="assets/code/page/name.cpp">Label</button>: code loaded from a file
  document.querySelectorAll("[data-code-src]").forEach(btn => {
    btn.addEventListener("click", () => {
      const src = btn.dataset.codeSrc;
      const language = CODE_LANGUAGES[src.split(".").pop().toLowerCase()];
      const content = codeBlock("Loading…");
      openModal({ title: btn.textContent.trim(), content });
      fetch(src)
        .then(r => (r.ok ? r.text() : Promise.reject(new Error(r.status))))
        .then(text => codeBlock(text, language))
        .catch(() => codeBlock(`// Code not added yet (${src}).`))
        .then(block => {
          highlightCode(block);
          content.replaceWith(block); // no-op if the popup was closed meanwhile
        });
    });
  });

  // <button data-blueprint="https://blueprintue.com/render/...">Label</button>: Unreal blueprint from blueprintUE.
  // Several space-separated URLs are shown stacked in the same popup.
  document.querySelectorAll("[data-blueprint]").forEach(btn => {
    btn.addEventListener("click", () => {
      const sources = btn.dataset.blueprint.trim().split(/\s+/).filter(Boolean);
      let content;
      if (sources.length) {
        content = document.createDocumentFragment();
        sources.forEach((src, i) => {
          const frame = content.appendChild(document.createElement("iframe"));
          frame.className = sources.length > 1 ? "blueprint stacked" : "blueprint";
          frame.src = src;
          frame.title = sources.length > 1 ? `${btn.textContent.trim()} ${i + 1}` : btn.textContent.trim();
          frame.allowFullscreen = true;
        });
      } else {
        content = codeBlock("// Blueprint not added yet.");
      }
      openModal({ title: btn.textContent.trim(), content, kind: "blueprint" });
    });
  });

  // <button data-code="id">Label</button>  +  <template id="code-id">...</template>
  document.querySelectorAll("[data-code]").forEach(btn => {
    btn.addEventListener("click", () => {
      const tpl = document.getElementById(`code-${btn.dataset.code}`);
      let content;
      if (tpl) {
        content = tpl.content.cloneNode(true);
        highlightCode(content);
      } else {
        content = document.createElement("pre");
        content.textContent = `// No <template id="code-${btn.dataset.code}"> found on this page.`;
      }
      openModal({ title: btn.textContent.trim(), content });
    });
  });
}

function initLightbox() {
  document.querySelectorAll(".gallery img, img.zoomable").forEach(img => {
    img.addEventListener("click", () => {
      if (img.classList.contains("missing-asset")) return;
      const big = document.createElement("img");
      big.src = img.dataset.full || img.src;
      big.alt = img.alt;
      openModal({ title: img.alt, content: big, kind: "image" });
    });
  });
}

/* ---------- Contributions nav scroll-spy ---------- */

function initScrollSpy() {
  const links = [...document.querySelectorAll(".contrib-nav a")];
  if (!links.length) return;
  const targets = links
    .map(a => document.getElementById(a.hash.slice(1)))
    .filter(Boolean);

  const update = () => {
    const y = window.scrollY + window.innerHeight * 0.35;
    let current = null;
    for (const t of targets) if (t.getBoundingClientRect().top + window.scrollY <= y) current = t;
    if (current && window.innerHeight + window.scrollY >= document.body.scrollHeight - 4) current = targets[targets.length - 1];
    links.forEach(a => a.classList.toggle("active", !!current && a.hash === `#${current.id}`));
  };
  window.addEventListener("scroll", update, { passive: true });
  update();
}

/* ---------- Back to top ---------- */

function initBackToTop() {
  const btn = document.querySelector(".back-to-top");
  if (!btn) return;
  const update = () => btn.classList.toggle("show", window.scrollY > 600);
  window.addEventListener("scroll", update, { passive: true });
  btn.addEventListener("click", () => window.scrollTo({ top: 0, behavior: "smooth" }));
  update();
}

/* ---------- Boot ---------- */

renderHeader();
renderFooter();
renderPager();
initAssetFallbacks();
initVideos();
initYouTube();
initCodePopups();
initLightbox();
initScrollSpy();
initBackToTop();
