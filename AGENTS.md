# AGENTS.md

# AI Agent Working Rules for Jherson Aguto Portfolio

This repository is a personal developer portfolio built with Blazor Server / ASP.NET Core, Entity Framework Core, PostgreSQL/Neon, CMS-managed portfolio content, GitHub integration, contact/collaboration features, and Render deployment.

The AI agent must work using a phased divide-and-conquer approach. Do not attempt large multi-file refactors in one step. Each phase must be planned, implemented, verified, and summarized before moving to the next phase.

## Core Objectives

The portfolio must remain:

- Professional and recruiter-ready
- Clean, modern, and developer-focused
- Fast enough for production deployment
- Mobile responsive
- Accessible
- SEO-friendly
- Consistent with the actual tech stack
- Compatible with existing CMS/admin functionality
- Compatible with current deployment configuration

## Non-Negotiable Rules

1. Do not rewrite the entire application unless explicitly requested.
2. Do not remove existing CMS/admin functionality.
3. Do not break GitHub integration.
4. Do not break contact or collaboration features.
5. Do not change the database provider or connection configuration without approval.
6. Do not hardcode dynamic CMS content unless approved.
7. Do not introduce unnecessary libraries or frameworks.
8. Do not overdesign the UI.
9. Do not make speculative changes without verifying the related files first.
10. Do not modify deployment-sensitive files unless the change is necessary and explained.
11. Always inspect existing code before editing.
12. Always summarize exact changed files after every phase.
13. Always run or recommend verification commands after every phase.
14. If unsure, stop and ask for review instead of guessing.
15. Treat public portfolio content as CMS/admin-managed unless proven otherwise.
16. Preserve dynamic content rendering from the system-level admin portal.
17. For SEO metadata, prefer CMS-driven values when available and use static values only as safe fallbacks.

## Divide-and-Conquer Workflow

Every task must follow this workflow:

1. Understand the requested goal.
2. Identify the smallest safe phase to work on.
3. Inspect only the relevant files for that phase.
4. Create a short phase plan.
5. Implement only that phase.
6. Verify the phase.
7. Summarize the changes.
8. Wait for approval before continuing to the next major phase.

Do not mix unrelated concerns in one edit. For example, do not combine SEO, project-card redesign, database seed cleanup, and contact form changes in one uncontrolled patch.

---

# Phase-Based Task Structure

## Phase 0: Context and Safety Audit

Purpose: Understand the repository before editing.

Required actions:

- Inspect project structure.
- Identify framework version and runtime assumptions.
- Inspect routing and layout files.
- Inspect CMS/admin pages.
- Inspect data models and database context.
- Inspect services such as content, GitHub, email/contact, and warm-up services.
- Inspect public-facing UI files.
- Inspect CSS and JavaScript assets.
- Inspect deployment-related files.
- Identify files that are safe to edit and files that are risky.

Expected output:

- Codebase overview
- Risk list
- Files likely to change
- Files that should not be touched without approval
- Recommended implementation phases

Do not edit code in Phase 0.

---

## Phase 1: Branding, Metadata, and SEO

Purpose: Fix public identity and link-preview quality before visual redesign.

Likely files:

- `Components/App.razor`
- `Components/Pages/Home.razor`
- `Components/Layout/MainLayout.razor`
- Possible shared head/meta components if they exist

Tasks:

- Update page title to a professional format:
  `Jherson Aguto | Full Stack Developer Portfolio`
- Improve meta description.
- Improve Open Graph metadata.
- Improve Twitter/X preview metadata if applicable.
- Replace generic branding such as `Portfolio` with `Jherson Aguto` where appropriate.
- Ensure metadata matches the public positioning:
  Full-stack developer, ASP.NET Core, Blazor, backend systems, databases, and real-world software projects.

Verification:

- Confirm page title renders correctly.
- Confirm no duplicate conflicting title/meta tags.
- Confirm layout still loads.
- Confirm no Blazor compilation errors.

Do not redesign the full UI in this phase.

---

## Phase 2: Content Consistency and Seed Data Cleanup

Purpose: Fix incorrect or inconsistent public content.

Likely files:

- `Data/AppDbContext.cs`
- `Components/Pages/Home.razor`
- CMS seed data files if separate
- README only if necessary

Tasks:

- Remove or correct outdated references to SQLite if the app uses PostgreSQL/Neon.
- Remove placeholder content such as `Acme Corp` if still present.
- Ensure portfolio project descriptions match the actual stack.
- Ensure footer tech stack is consistent.
- Ensure domain references use `jhersonaguto.dev` where appropriate.
- Review project descriptions for clarity.
- Use a recruiter-friendly format:
  Problem → Solution → Tech → Result.

Verification:

- Confirm seed data still compiles.
- Confirm EF Core model/data initialization is not broken.
- Confirm project cards still render correctly.
- Confirm CMS-driven content is preserved.

Do not change database provider configuration in this phase.

---

## Phase 3: Navigation, Hero, and First Impression UX

Purpose: Improve the first 5 seconds of the visitor experience.

Likely files:

- `Components/Pages/Home.razor`
- `Components/Layout/MainLayout.razor`
- `wwwroot/css/portfolio.css`

Tasks:

- Improve hero headline and subheadline.
- Make the primary call-to-action clear.
- Make Resume and Projects buttons more visible.
- Improve spacing and visual hierarchy.
- Ensure the navbar brand is professional.
- Ensure mobile navigation remains usable.
- Avoid heavy animations that hurt mobile performance.

Verification:

- Check desktop layout.
- Check mobile layout.
- Check header behavior.
- Check CTA links.
- Confirm no CSS regression in admin pages.

Do not touch project cards or skill visualization in this phase unless required by hero layout.

---

## Phase 4: Projects Section Refactor

Purpose: Make projects more convincing for recruiters and technical reviewers.

Likely files:

- `Components/Pages/Home.razor`
- `wwwroot/css/portfolio.css`
- Project model/CMS files only if a new property is required and approved

Tasks:

- Improve project card layout.
- Make top/featured projects more prominent if possible.
- Improve tech tag readability.
- Make GitHub/live demo links clearer.
- Ensure long descriptions do not break card layout.
- Prefer better rendering of existing CMS data before changing the data model.
- If adding a `Featured` field requires database/model changes, stop and request approval first.

Recommended top project emphasis:

- Portfolio CMS
- GAS Forecasting Backend
- Real-time/video/backend-heavy project if available

Verification:

- Confirm all project cards render.
- Confirm links work.
- Confirm mobile project cards stack cleanly.
- Confirm CMS data still controls project content.

Do not add database migrations unless explicitly approved.

---

## Phase 5: Skills and Tech Stack UX

Purpose: Make skills understandable to both technical and non-technical recruiters.

Likely files:

- `Components/Pages/Home.razor`
- `wwwroot/css/portfolio.css`
- Existing skill visualization files if separate

Tasks:

- Preserve the existing visual identity if it is unique.
- Add or improve text-based skill grouping.
- Make categories clear:
  Backend, Frontend, Database, DevOps/Tools, Data/AI, IoT if applicable.
- Ensure the skills section is accessible without relying only on visuals.
- Avoid exaggerated proficiency claims.

Verification:

- Confirm visual skill section still works.
- Confirm text skills are readable on mobile.
- Confirm no duplicate confusing skill labels.
- Confirm accessibility is improved.

---

## Phase 6: Experience and About Section Polish

Purpose: Improve professional storytelling.

Likely files:

- `Components/Pages/Home.razor`
- `Data/AppDbContext.cs` if seeded content needs correction
- CMS content files if applicable

Tasks:

- Make internship experience concise and impact-focused.
- Highlight backend development, NestJS, TypeScript, Prisma, Supabase, DTOs, services, controllers, database workflows, and real-time systems where accurate.
- Improve About section to sound specific and credible.
- Avoid generic claims like “passionate developer” unless supported by concrete work.

Verification:

- Confirm content is professional.
- Confirm no grammar issues.
- Confirm timeline layout remains stable.
- Confirm mobile spacing is clean.

---

## Phase 7: Contact, Resume, and Collaboration UX

Purpose: Make conversion actions reliable and professional.

Likely files:

- `Components/Pages/Home.razor`
- Contact/collaboration components if separate
- Contact/email services only if a bug is found
- `wwwroot/css/portfolio.css`

Tasks:

- Make resume link direct and professional if possible.
- Keep resume CMS-driven unless approved otherwise.
- Improve contact form styling consistency.
- Ensure collaboration/network section does not overpower recruiter-focused content.
- Consider moving collaboration lower on the page if needed.
- Check external links for safe attributes:
  `target="_blank"` with `rel="noopener noreferrer"`.

Verification:

- Confirm contact form still works.
- Confirm collaboration submission still works.
- Confirm resume button works.
- Confirm all external links are safe.

Do not alter email provider logic unless necessary.

---

## Phase 8: Accessibility and Responsive QA

Purpose: Ensure the site is usable on common devices and accessible.

Likely files:

- `Components/Pages/Home.razor`
- `Components/Layout/MainLayout.razor`
- `wwwroot/css/portfolio.css`

Tasks:

- Check heading order.
- Check button/link labels.
- Check alt text for meaningful images.
- Check keyboard navigation.
- Check color contrast.
- Check mobile spacing.
- Check overflow issues.
- Reduce motion where appropriate.
- Ensure focus states are visible.

Verification:

- Desktop viewport check.
- Tablet viewport check.
- Mobile viewport check.
- Keyboard-only navigation check.
- Reduced motion check if applicable.

---

## Phase 9: Performance and Maintainability

Purpose: Improve long-term maintainability without risky rewrites.

Likely files:

- `Components/Pages/Home.razor`
- `wwwroot/css/portfolio.css`
- Component files if extraction is safe

Tasks:

- Identify large components that can be safely split.
- Identify unused CSS.
- Reduce duplicate markup.
- Improve class naming if necessary.
- Optimize images/assets if applicable.
- Avoid premature abstraction.
- Do not split components if it risks breaking CMS data flow.

Verification:

- Run build.
- Confirm all sections render.
- Confirm CSS still applies correctly.
- Confirm no runtime errors.

---

## Phase 10: Final Verification and Deployment Readiness

Purpose: Confirm the portfolio is safe to deploy.

Required checks:

- `dotnet build`
- Run locally if possible.
- Check public home page.
- Check admin/CMS pages.
- Check project rendering.
- Check GitHub integration.
- Check contact/collaboration form.
- Check resume link.
- Check mobile layout.
- Check SEO metadata.
- Check external links.
- Check deployment config.
- Confirm no secrets were exposed.
- Confirm no unrelated files were changed.

Expected output:

- Final summary
- Changed files list
- Before/after explanation
- Verification result
- Known limitations
- Recommended next steps

---

# Phase Completion Format

After each phase, the AI agent must respond using this format:

## Phase Completed: [Phase Name]

### Files Changed
- `path/to/file`
  - What changed
  - Why it changed

### Verification Performed
- Build result:
- UI check:
- Mobile check:
- Functional check:

### Risks or Notes
- Any remaining concern

### Next Recommended Phase
- Phase number and title

Do not continue to the next phase unless the user approves or explicitly asks to proceed.

---

# Anti-Hallucination Rules

The AI agent must:

- Use actual file contents as the source of truth.
- Never assume a file exists without checking.
- Never invent routes, services, models, or components.
- Never claim a test passed unless it was actually run.
- Never claim a deployment succeeded unless it was actually deployed.
- Never modify unrelated files.
- Never make broad architectural changes without approval.
- Prefer small, reversible changes.
- Ask for approval before database schema changes.
- Ask for approval before changing authentication, admin, email, or deployment behavior.
- Clearly say when something was not verified.

---

# Safe Editing Strategy

For every implementation phase:

1. Read relevant files first.
2. Identify minimal patch.
3. Apply focused edits.
4. Avoid formatting entire files unless necessary.
5. Avoid touching generated files.
6. Avoid changing secrets or environment variables.
7. Run build or explain why build was not run.
8. Summarize exactly what changed.

---

# Project-Specific Priorities

Prioritize these issues first:

1. Fix branding from generic `Portfolio` to `Jherson Aguto`.
2. Fix SEO title and metadata.
3. Fix SQLite vs PostgreSQL/Neon content inconsistency.
4. Fix placeholder company/content references.
5. Improve project descriptions.
6. Improve hero section.
7. Improve project card UX.
8. Improve skills readability.
9. Improve resume/contact flow.
10. Improve accessibility and responsive layout.
11. Improve maintainability only after visual/content issues are stable.

---

# Domain, Subdomain, DNS, and HTTPS Rules

This project uses the custom domain:

`jhersonaguto.dev`

The AI agent must treat domain and DNS-related changes as deployment-sensitive.

## Domain Ownership Rule

The owned root domain is:

`jhersonaguto.dev`

Valid subdomains must be created under this root domain.

Examples of valid subdomains:

- `resume.jhersonaguto.dev`
- `api.jhersonaguto.dev`
- `admin.jhersonaguto.dev`
- `projects.jhersonaguto.dev`
- `labs.jhersonaguto.dev`
- `gas.jhersonaguto.dev`
- `chatzoom.jhersonaguto.dev`

Invalid assumption example:

- `jhersonaguto.resume.dev`

Do not suggest or configure `jhersonaguto.resume.dev` unless the user also owns or controls `resume.dev`.

## HTTPS Requirement

Because this project uses a `.dev` domain, all public root-domain and subdomain URLs must be treated as HTTPS-only.

Use:

`https://`

Do not use:

`http://`

for production `.dev` URLs.

## Allowed Domain References

The primary public portfolio URL is:

`https://jhersonaguto.dev`

The canonical homepage URL is:

`https://jhersonaguto.dev/`

The sitemap URL is:

`https://jhersonaguto.dev/sitemap.xml`

The robots.txt URL is:

`https://jhersonaguto.dev/robots.txt`

The health check URL may use:

`https://jhersonaguto.dev/health`

only if confirmed to be used purely for health-check/keep-alive behavior.

## Resume Subdomain Rule

If the user wants a dedicated resume subdomain, prefer:

`resume.jhersonaguto.dev`

Possible use cases:

- Dedicated resume landing page
- Direct downloadable resume page
- Redirect to `/resume.pdf`
- Static resume microsite

Do not implement this automatically. Adding a resume subdomain requires approval because it may involve DNS, Render custom domain configuration, routing, certificates, and deployment settings.

## Admin Subdomain Rule

If the user wants an admin subdomain, prefer:

`admin.jhersonaguto.dev`

But do not expose admin functionality publicly without confirming authentication, authorization, and deployment security.

Any admin subdomain change requires explicit approval.

## API Subdomain Rule

If the user wants a backend/API subdomain, prefer:

`api.jhersonaguto.dev`

Do not change API base URLs, CORS policies, environment variables, or deployment settings without approval.

## DNS Change Approval Gate

The AI agent must request explicit approval before making or recommending implementation changes involving:

- DNS records
- CNAME records
- A records
- Render custom domains
- SSL certificate setup
- Canonical domain redirects
- Root-to-www redirects
- Subdomain routing
- Admin subdomain exposure
- API subdomain exposure
- Resume subdomain setup
- Environment variables containing public URLs
- KeepAlive/BaseUrl changes
- OAuth callback URLs
- CORS origin lists
- Email/domain verification records

## Safe Recommendation Format

When suggesting domain or subdomain changes, use this format:

### Proposed Domain Change

- Domain/subdomain:
- Purpose:
- Required DNS record:
- Required Render/custom-domain setting:
- HTTPS/certificate requirement:
- App/router change needed:
- Environment variable change needed:
- Security risk:
- Approval required before implementation: Yes

## Do Not Assume

The AI agent must not assume:

- A DNS record already exists
- A Render custom domain is already configured
- SSL is already issued
- A subdomain route already exists in the app
- A resume file already exists at `/resume.pdf`
- Admin pages are safe to expose through a public subdomain
- API routes are ready for cross-origin requests
- Google indexing will happen immediately after metadata changes

## SEO Domain Consistency

For SEO, prefer one canonical identity:

`https://jhersonaguto.dev/`

Avoid creating duplicate public content on multiple subdomains unless canonical tags or redirects are planned.

For example:

- Main portfolio should stay on `jhersonaguto.dev`.
- Resume-specific content may use `resume.jhersonaguto.dev` only if it is intentionally separate.
- Do not duplicate the same homepage content on multiple subdomains.

---

---

# Approval Gates

The AI agent must request approval before:

- Adding database migrations
- Changing database provider/configuration
- Adding new dependencies
- Rewriting major components
- Removing CMS/admin features
- Changing deployment behavior
- Changing email/contact service behavior
- Replacing the visual identity entirely
- Hardcoding content that is currently CMS-managed
- DNS record changes
- Custom domain/subdomain setup
- Render domain/certificate configuration
- Canonical redirect changes
- CORS origin changes
- OAuth/callback URL changes
- Admin subdomain exposure
- API subdomain exposure
- Resume subdomain implementation

---

# Preferred Result

The final portfolio should feel:

- Personal, not generic
- Clean, not empty
- Modern, not overdesigned
- Technical, but understandable
- Professional enough for internship/job applications
- Consistent with Jherson Aguto’s actual skills and projects
- Stable for deployment on Render with PostgreSQL/Neon

End of AGENTS.md.
