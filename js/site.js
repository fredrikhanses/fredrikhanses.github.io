/* ==========================================================================
   Shared layout + behaviour for every page.
   Pages only need:  <div id="site-header"></div> ... <div id="site-footer"></div>
   ========================================================================== */

const SITE = {
  name: "Fredrik Hanses",
  title: "Game Programmer",
  location: "Stockholm",
  email: "fredrik.hanses@gmail.com",
  phone: "+46762756925",
  phoneDisplay: "+46 76 275 69 25",
  linkedin: "https://www.linkedin.com/in/fredrikhanses/",
  github: "https://github.com/fredrikhanses",
  cv: "assets/cv.pdf",
};

// Projects grouped by studio/school, in the order shown in the "Game Projects" dropdown.
const PROJECT_GROUPS = [
  {
    name: "Resolution Games",
    projects: [
      { slug: "battlemarked", name: "Demeo x Dungeons & Dragons: Battlemarked" },
      { slug: "demeo",        name: "Demeo" },
      { slug: "demeobattles", name: "Demeo Battles" },
      { slug: "grimkeep",     name: "GrimKeep" },
    ],
  },
  {
    name: "Futuregames",
    projects: [
      { slug: "marenostrum",   name: "Mare Nostrum" },
      { slug: "alter",         name: "Alter" },
      { slug: "arrowhood",     name: "Arrowhood" },
      { slug: "seventhcircle", name: "Seventh Circle" },
      { slug: "projectbrawl",  name: "Project Brawl" },
      { slug: "bullethell",    name: "Bullet Hell" },
    ],
  },
  {
    name: "KTH",
    projects: [
      { slug: "sumodash", name: "Sumo Dash" },
    ],
  },
];
const PROJECTS = PROJECT_GROUPS.flatMap(g => g.projects);

const ICONS = {
  linkedin: '<svg viewBox="0 0 24 24"><path d="M4.98 3.5a2.5 2.5 0 1 1 0 5 2.5 2.5 0 0 1 0-5zM3 9h4v12H3zM9 9h3.8v1.7h.05c.53-1 1.83-2.05 3.77-2.05C20.6 8.65 21 11.2 21 14.5V21h-4v-5.8c0-1.4-.03-3.2-1.95-3.2-1.95 0-2.25 1.52-2.25 3.1V21H9z"/></svg>',
  github: '<svg viewBox="0 0 24 24"><path d="M12 .5a11.5 11.5 0 0 0-3.64 22.41c.58.1.79-.25.79-.56v-2c-3.2.7-3.88-1.37-3.88-1.37-.53-1.33-1.28-1.69-1.28-1.69-1.05-.71.08-.7.08-.7 1.16.08 1.77 1.19 1.77 1.19 1.03 1.77 2.7 1.26 3.36.96.1-.75.4-1.26.73-1.55-2.55-.29-5.24-1.28-5.24-5.69 0-1.26.45-2.28 1.19-3.09-.12-.29-.52-1.46.11-3.05 0 0 .97-.31 3.17 1.18a11 11 0 0 1 5.77 0c2.2-1.49 3.17-1.18 3.17-1.18.63 1.59.23 2.76.11 3.05.74.81 1.19 1.83 1.19 3.09 0 4.42-2.7 5.4-5.26 5.68.41.36.78 1.06.78 2.14v3.17c0 .31.21.67.8.56A11.5 11.5 0 0 0 12 .5z"/></svg>',
  mail: '<svg viewBox="0 0 24 24"><path d="M2 5h20v14H2zm2 2v.4l8 5.1 8-5.1V7zm16 2.75-8 5.1-8-5.1V17h16z"/></svg>',
  phone: '<svg viewBox="0 0 24 24"><path d="M6.6 10.8a15.1 15.1 0 0 0 6.6 6.6l2.2-2.2c.3-.3.7-.4 1-.2 1.1.4 2.3.6 3.6.6.6 0 1 .4 1 1V20c0 .6-.4 1-1 1A17 17 0 0 1 3 4c0-.6.4-1 1-1h3.5c.6 0 1 .4 1 1 0 1.3.2 2.5.6 3.6.1.3 0 .7-.2 1z"/></svg>',
  soundOn: '<svg viewBox="0 0 24 24"><path d="M3 9v6h4l5 5V4L7 9zm13.5 3A4.5 4.5 0 0 0 14 8v8a4.5 4.5 0 0 0 2.5-4zM14 3.2v2.1a7 7 0 0 1 0 13.4v2.1a9 9 0 0 0 0-17.6z"/></svg>',
  soundOff: '<svg viewBox="0 0 24 24"><path d="M16.5 12A4.5 4.5 0 0 0 14 8v2.2l2.45 2.45c.03-.2.05-.42.05-.65zM19 12a7 7 0 0 1-.55 2.7l1.5 1.5A8.9 8.9 0 0 0 21 12a9 9 0 0 0-7-8.8v2.1A7 7 0 0 1 19 12zM4.3 3 3 4.3 7.7 9H3v6h4l5 5v-6.7l4.25 4.25A7 7 0 0 1 14 18.7v2.1a9 9 0 0 0 3.7-1.8l2 2L21 19.7l-9-9zM12 4 9.9 6.1 12 8.2z"/></svg>',
};

const currentPage = location.pathname.split("/").pop().replace(".html", "") || "index";

/* ---------- Header / footer ---------- */

function renderHeader() {
  const el = document.getElementById("site-header");
  if (!el) return;
  const isProject = PROJECTS.some(p => p.slug === currentPage) || currentPage === "game-projects";
  el.outerHTML = `
  <header class="site-header">
    <div class="container">
      <a class="brand" href="index.html">
        <img class="brand-avatar" src="assets/images/avatar.jpg" alt="${SITE.name}">
        <div>
          <div class="brand-name">${SITE.name}</div>
          <div class="brand-sub"><span>${SITE.title}</span><span>${SITE.location}</span></div>
        </div>
      </a>
      <button class="menu-button" aria-label="Open menu" aria-expanded="false"><span></span></button>
      <nav class="main-nav" aria-label="Main">
        <ul>
          <li class="has-dropdown">
            <a href="game-projects.html" class="nav-toggle ${isProject ? "active" : ""}" aria-haspopup="true">Game Projects</a>
            <ul class="dropdown">
              ${PROJECT_GROUPS.map(g => `
                <li class="dropdown-heading">${g.name}</li>
                ${g.projects.map(p => `<li><a href="${p.slug}.html" class="${p.slug === currentPage ? "active" : ""}">${p.name}</a></li>`).join("")}
              `).join("")}
            </ul>
          </li>
          <li><a href="about.html" class="${currentPage === "about" ? "active" : ""}">About me</a></li>
          <li><a href="${SITE.cv}" target="_blank" rel="noopener">CV</a></li>
        </ul>
      </nav>
      ${socialLinks()}
    </div>
  </header>`;
}

function socialLinks() {
  return `
  <div class="social">
    <a href="${SITE.linkedin}" target="_blank" rel="noopener" aria-label="LinkedIn">${ICONS.linkedin}</a>
    <a href="${SITE.github}" target="_blank" rel="noopener" aria-label="GitHub">${ICONS.github}</a>
    <a href="mailto:${SITE.email}?subject=Interesting%20Portfolio" aria-label="Email">${ICONS.mail}</a>
    <a href="tel:${SITE.phone}" aria-label="Phone">${ICONS.phone}</a>
  </div>`;
}

function renderFooter() {
  const el = document.getElementById("site-footer");
  if (!el) return;
  el.outerHTML = `
  <footer class="site-footer">
    <a href="${SITE.linkedin}" target="_blank" rel="noopener">&copy;${new Date().getFullYear()} ${SITE.name}</a>
  </footer>
  <button class="back-to-top" aria-label="Back to top">&uarr;</button>`;
}

function initNav() {
  const header = document.querySelector(".site-header");
  if (!header) return;
  const menuBtn = header.querySelector(".menu-button");
  menuBtn.addEventListener("click", () => {
    const open = header.classList.toggle("nav-open");
    menuBtn.setAttribute("aria-expanded", open);
    document.body.classList.toggle("modal-open", open);
  });
  // The "Game Projects" dropdown opens on hover/focus (CSS); clicking the label opens game-projects.html.
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
  // Feature videos: autoplay muted while on screen, with a sound toggle (like the Wix player).
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
      <img src="https://i.ytimg.com/vi/${id}/hqdefault.jpg" alt="${title}" loading="lazy">
      <button class="youtube-play" aria-label="Play ${title}"></button>`;
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

  // <button data-blueprint="https://blueprintue.com/render/...">Label</button>: Unreal blueprint from blueprintUE
  document.querySelectorAll("[data-blueprint]").forEach(btn => {
    btn.addEventListener("click", () => {
      const src = btn.dataset.blueprint;
      let content;
      if (src) {
        content = document.createElement("iframe");
        content.className = "blueprint";
        content.src = src;
        content.title = btn.textContent.trim();
        content.allowFullscreen = true;
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

/* ---------- Section nav scroll-spy ---------- */

function initScrollSpy() {
  const links = [...document.querySelectorAll(".section-nav a")];
  if (!links.length) return;
  const targets = links
    .map(a => document.getElementById(a.hash.slice(1)))
    .filter(Boolean);

  const update = () => {
    const y = window.scrollY + window.innerHeight * 0.35;
    let current = targets[0];
    for (const t of targets) if (t.offsetTop <= y) current = t;
    if (window.innerHeight + window.scrollY >= document.body.scrollHeight - 4) current = targets[targets.length - 1];
    links.forEach(a => a.classList.toggle("active", a.hash === `#${current.id}`));
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
initNav();
initAssetFallbacks();
initVideos();
initYouTube();
initCodePopups();
initLightbox();
initScrollSpy();
initBackToTop();
