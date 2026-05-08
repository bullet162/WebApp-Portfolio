Use this full prompt for **Antigravity**. It tells it to revise Phase 3 into the full **Developer Network Refactor** with Gmail SMTP notifications, magic edit links, AI-assisted enrichment, citations, moderation, migration safety, and one extra enhancement I recommend: **Reciprocal Link Verification**.

We are changing the next major phase of this portfolio project.

The next active phase must be:

# Phase 3: Developer Network Refactor

This replaces the previous Phase 3 Hero/Nav polish priority.

The goal is to transform the current bottom-page “Collaboration Network” into a professional, interactive, CMS/admin-managed Developer Network / Portfolio Network feature.

This should make my portfolio feel like a real full-stack web application, not just a static resume site.

Important: follow `AGENTS.md` strictly, but for this specific Phase 3 feature, a larger refactor and additive database migrations are allowed because this is now the approved major feature direction.

Do not implement immediately until you inspect the current codebase and produce the full revised implementation plan.

---

# Core Product Direction

The current Collaboration Network feels like a static list of submitted names/messages at the bottom of the portfolio.

I want to refactor it into a professional Developer Network where approved developers, classmates, collaborators, and portfolio builders can appear as curated public profile cards connected to my portfolio.

The feature should support:

- Public Developer Network directory
- Admin moderation
- Manual approval/rejection/hiding/featuring/verifying
- Public profile pages
- Search and filters
- Gmail SMTP notifications
- Email-based magic edit links
- Pending profile revisions instead of instant public edits
- AI-assisted public profile enrichment using Gemini API
- Source/citation links for AI-generated details
- Consent controls
- Data preservation for old collaboration records
- No public exposure of private emails
- Optional badge/snippet for collaborators
- Optional reciprocal link verification

---

# Existing Data Safety Rule

Do not damage or delete old Collaboration Network data.

Do not drop old columns.
Do not rename existing columns destructively.
Do not delete records.
Do not replace old records with new records.
Do not assume old records are disposable.

Use additive migrations only unless I explicitly approve destructive changes.

Old collaboration records should remain valid after the refactor.

If old records do not have new fields, backfill them safely using defaults.

Example safe defaults:

- `Status = Approved` or `Pending` depending on how current visibility works
- `IsVisible = true` if they are already publicly visible
- `IsFeatured = false`
- `IsVerified = false`
- `OpenToCollaborate = false` unless explicitly known
- `AiEnrichmentConsent = false`
- Generate `PublicSlug` from existing name if possible
- Keep portfolio URL, name, and message unchanged

Before migration, report how existing records will be preserved.

---

# Route and Domain Direction

Do not create a subdomain yet.

Use the main domain path:

- `/network`
- `/network/{slug}`
- `/network/join` if needed
- `/network/edit-request` if needed
- `/network/manage/{token}` or equivalent secure magic edit route

Do not use:

- `network.jhersonaguto.dev`

A subdomain may be considered later only if the Developer Network becomes a standalone platform.

The current recommendation is:

`https://jhersonaguto.dev/network`

---

# Authentication Direction

Do not require full account registration yet.

Do not build a full auth system for collaborators.

Use email-based magic edit links.

Reason:
Requiring full registration adds too much friction and requires password reset, login, account verification, user dashboards, session management, and account security. That is unnecessary for this portfolio feature.

Recommended flow:

1. Developer submits request.
2. Request is saved as pending.
3. Confirmation email is sent.
4. Admin reviews request.
5. Admin approves/rejects/hides/features/verifies.
6. If approved, profile appears publicly.
7. Developer can request an edit link using their submitted email.
8. System sends a secure time-limited edit link.
9. Developer submits edits.
10. Edits are saved as a pending revision.
11. Admin reviews current vs proposed changes.
12. Public profile updates only after admin approval.

No instant public self-editing.

---

# Email System Direction

Use Gmail SMTP with app password.

Do not commit credentials.
Do not hardcode credentials.
Use environment variables only.

Suggested environment variables:

```env
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=
SMTP_APP_PASSWORD=
SMTP_FROM_NAME=Jherson Aguto
SMTP_FROM_EMAIL=
SMTP_ENABLE_SSL=true
```

Email events to support:

1. New Developer Network request received by submitter.
2. Admin notification for new request.
3. Request approved.
4. Request rejected or hidden.
5. Edit link requested.
6. Profile edit submitted and pending review.
7. Profile revision approved.
8. Profile revision rejected.
9. AI enrichment generated and pending admin review, if useful.
10. AI enrichment approved, if useful.

Important email safety:

* Do not expose SMTP credentials.
* Do not log app passwords.
* Rate-limit edit link requests.
* Do not reveal whether an email exists when requesting an edit link.
* Use generic response:
  “If this email exists in the Developer Network, an edit link has been sent.”

---

# Magic Edit Link Direction

Use secure magic edit tokens.

Rules:

* Generate cryptographically secure random tokens.
* Store only token hash in the database, not raw token.
* Token should expire, recommended: 30 minutes to 2 hours.
* Token should be single-use if feasible.
* Token should allow editing only the matching profile.
* Do not expose token in logs.
* Do not expose token publicly.
* After edit submission, changes should go to pending review.

Suggested fields:

* `EditTokenHash`
* `EditTokenExpiresAt`
* `EditTokenUsedAt`
* `LastEditRequestedAt`
* `LastEditedAt`

If a separate token table is cleaner, propose it.

---

# Pending Revision Direction

Do not let collaborators instantly overwrite their public profile.

Use a pending revision workflow.

Recommended table:

`DeveloperNetworkProfileRevision`

Possible fields:

* `Id`
* `CollaborationRequestId` or `DeveloperProfileId`
* `ProposedFirstName`
* `ProposedLastName`
* `ProposedPortfolioUrl`
* `ProposedMessage`
* `ProposedRoleTitle`
* `ProposedConnectionType`
* `ProposedTags`
* `ProposedGitHubUrl`
* `ProposedLinkedInUrl`
* `ProposedOpenToCollaborate`
* `Status`
* `SubmittedAt`
* `ReviewedAt`
* `ReviewedBy`
* `ReviewNote`

Possible status values:

* `Pending`
* `Approved`
* `Rejected`

Admin should see:

* Current value
* Proposed value
* Difference/change indicator
* Approve/reject buttons

Public profile should not change until revision is approved.

---

# Developer Network Public UX

The Developer Network should feel curated, interactive, and professional.

Recommended section/page structure:

## Developer Network

Description:

“A curated list of developers, classmates, collaborators, and portfolio builders connected with me.”

Controls:

* Search input:
  `Search by name, skill, message, portfolio, or role...`

Filter chips:

* All
* Featured
* Verified
* Open to Collaborate
* Frontend
* Backend
* C#
* Blazor
* React
* Student Developer
* Portfolio Exchange

Only include filters that can be supported by existing or new data.

Public cards should show, when available:

* Name
* Initial/avatar
* Role/title
* Connection type
* Tags/skills/interests
* Short message or intro
* Portfolio button
* GitHub button if provided
* Verified badge
* Featured badge
* Open-to-collaborate badge
* Last reviewed date if available
* AI-assisted summary indicator if applicable

Public cards must not show:

* Email address
* Edit token
* Internal notes
* Admin-only status
* Raw AI prompt
* Private errors
* Sensitive inferred data

Move the Developer Network higher in the portfolio. It should not be buried at the bottom.

Recommended page order later:

1. Hero
2. Experience
3. Projects
4. Tech Stack
5. GitHub
6. Developer Network
7. Contact / Join My Network
8. Footer

Do not refactor unrelated sections unless needed to reposition the network.

---

# Join My Network UX

Keep the existing collaboration request form behavior, but reframe it as:

`Join My Network`

Suggested form fields:

* First name
* Last name
* Email
* Portfolio URL
* GitHub URL, optional
* LinkedIn/professional URL, optional
* Role/title, optional
* Connection type, optional
* Tags/skills/interests, optional
* Collaboration idea / message
* Open to collaborate checkbox
* Public display consent checkbox
* AI enrichment consent checkbox
* Email notification/edit-link consent checkbox

Consent text should be clear.

Suggested public display consent:

“By submitting, I allow Jherson Aguto to review and display my submitted public profile information on the Developer Network if approved. My email will be used only for review, notifications, and edit-link access, and will not be shown publicly.”

Suggested AI enrichment consent:

“I allow this portfolio system to analyze my submitted public portfolio/professional links to suggest Developer Network profile details. AI-generated details will be reviewed before publishing.”

If AI consent is not given:

* Save request normally.
* Do not run AI enrichment.

---

# Public Card Preview Before Submit

Add a public card preview before submission if feasible.

The submitter should see how their card may appear publicly:

* Name
* Role/title
* Tags
* Message
* Portfolio button
* Open to collaborate badge if selected

This reduces messy submissions and makes the feature feel polished.

If this is too much for first implementation, include it as a sub-phase.

---

# Profile Completeness Enhancement

Add a profile completeness concept.

This may be internal/admin-only at first.

Completeness can be calculated from:

* Name exists
* Portfolio URL exists
* Message/intro exists
* Role/title exists
* Tags exist
* GitHub/professional link exists
* Approved status
* Verified status
* AI summary approved

Example:

`Profile completeness: 80%`

Use this to encourage better profiles and help admin prioritize incomplete records.

Do not make it overly complex.

---

# AI-Assisted Developer Profile Enrichment

Add AI-assisted public profile enrichment using Gemini API.

This is a draft/review-only system.

Do not auto-publish AI-generated content.

Use Gemini only through environment variables.

Suggested environment variables:

```env
GEMINI_API_KEY=
GEMINI_MODEL=gemini-2.5-flash
AI_ENRICHMENT_ENABLED=true
AI_ENRICHMENT_REQUIRE_CONSENT=true
AI_ENRICHMENT_REQUIRE_ADMIN_APPROVAL=true
```

The AI may analyze only submitted public/professional sources:

* Submitted portfolio URL
* Submitted GitHub URL
* Submitted LinkedIn/professional URL
* Other professional links explicitly submitted by the user

The AI should suggest:

* Role/title
* Short developer bio
* Skills/tags
* Project highlights
* Collaboration interests
* Source links/citations
* Confidence level

The AI must not:

* Invent facts
* Include unsupported claims
* Include sensitive personal information
* Include unrelated personal information
* Include email publicly
* Include phone number publicly
* Include home address publicly
* Infer religion, politics, health, ethnicity, or other sensitive traits
* Scrape unrelated personal data
* Publish anything without admin review

AI output should be structured JSON.

Suggested shape:

```json
{
  "headline": "",
  "summary": "",
  "skills": [
    {
      "name": "",
      "confidence": 0.0,
      "sources": [
        {
          "label": "",
          "url": ""
        }
      ]
    }
  ],
  "projectHighlights": [
    {
      "title": "",
      "description": "",
      "sources": [
        {
          "label": "",
          "url": ""
        }
      ]
    }
  ],
  "collaborationInterests": [],
  "overallConfidence": 0.0,
  "warnings": []
}
```

Rules:

* Every generated claim should have at least one source URL.
* If a claim cannot be sourced, omit it or return `unknown`.
* Keep summaries concise and professional.
* Store citations/sources with the generated enrichment.
* Admin must approve enrichment before it becomes public.

Recommended AI enrichment statuses:

* `NotEnriched`
* `EnrichmentPending`
* `EnrichmentReady`
* `EnrichmentApproved`
* `EnrichmentRejected`
* `EnrichmentFailed`

---

# AI Enrichment Data Model

Prefer a separate table for AI output.

Recommended table:

`DeveloperProfileEnrichment`

Possible fields:

* `Id`
* `CollaborationRequestId` or `DeveloperProfileId`
* `Status`
* `GeneratedHeadline`
* `GeneratedSummary`
* `GeneratedSkillsJson`
* `GeneratedProjectHighlightsJson`
* `GeneratedCollaborationInterestsJson`
* `SourceCitationsJson`
* `ConfidenceScore`
* `ModelUsed`
* `PromptVersion`
* `GeneratedAt`
* `ApprovedAt`
* `RejectedAt`
* `ApprovedBy`
* `ErrorMessage`

Why separate table:

* Existing submitted data remains safe.
* AI output is isolated.
* AI output can be regenerated.
* Admin can compare submitted data vs AI suggestions.
* Sources/citations can be audited.
* Failed AI runs do not damage public profiles.

---

# AI Citation / Source UX

Public profile pages may show a “Sources” section.

Example:

Sources:

* Portfolio
* GitHub
* LinkedIn

For small cards, do not overload the UI with citations.

For individual profile pages, show detailed source links.

Add a small transparency label where appropriate:

“AI-assisted profile details are based on submitted public links and reviewed before publishing.”

---

# Admin Portal Requirements

Admin should be able to:

* View all Developer Network requests.
* See pending, approved, rejected, hidden, featured, verified records.
* Approve request.
* Reject request.
* Hide request.
* Feature/unfeature profile.
* Verify/unverify profile.
* Edit role/title, tags, connection type, display order.
* View submitted email privately.
* View AI consent status.
* Generate AI enrichment if consent exists.
* View AI-generated draft data.
* View source citations.
* Approve/reject AI enrichment.
* Regenerate AI enrichment.
* View profile completeness.
* View pending profile revisions.
* Compare current vs proposed values.
* Approve/reject profile revisions.
* Add review notes.

Public emails must remain admin-only.

---

# Audit Log Enhancement

Add lightweight audit logging if feasible.

Suggested fields either on profile/revision records or a separate audit table:

* `ReviewedBy`
* `ReviewedAt`
* `ReviewNote`
* `LastStatusChangedAt`
* `LastStatusChangedBy`
* `LastStatusChangeReason`

If a separate audit table is too much for Phase 3, include it as a later sub-phase.

---

# Abuse Protection

Because this feature includes public forms and email sending, add basic abuse protection.

Recommended:

* Honeypot field on public forms
* Rate limit submissions per IP/email if feasible
* Rate limit edit-link requests per email
* Cooldown between edit-link email requests
* Generic response for edit-link request
* Server-side validation for URLs
* Max character limits for fields
* Sanitize display content
* Ensure external links use:
  `target="_blank"`
  `rel="noopener noreferrer"`

Do not add heavy CAPTCHA unless necessary. If CAPTCHA requires new dependencies or external setup, recommend it but do not implement without approval.

---

# Extra Enhancement: Reciprocal Link Verification

Add this as an optional enhancement.

Purpose:
A collaborator can optionally add a badge or link on their own portfolio pointing back to my Developer Network. My site can verify that public reciprocal link.

This creates a lightweight portfolio ring without forcing other people to modify their codebase.

Badge example:

```html
<a href="https://jhersonaguto.dev/network">
  Connected with Jherson Aguto
</a>
```

Optional public badge:

`Reciprocal Link Verified`

How it works:

1. My site gives collaborator a copyable badge snippet.
2. Collaborator may add it to their portfolio.
3. Admin can click “Verify Reciprocal Link.”
4. System checks their submitted portfolio URL for a link to:
   `https://jhersonaguto.dev`
   or
   `https://jhersonaguto.dev/network`
5. If found, mark:
   `ReciprocalLinkVerified = true`
   `ReciprocalLinkVerifiedAt = DateTime.UtcNow`

Rules:

* Do not require reciprocal links.
* Do not penalize collaborators who do not add it.
* Do not modify other people’s portfolios.
* Do not crawl unrelated pages aggressively.
* Check only the submitted portfolio URL or explicitly submitted professional URL.
* Make verification manual/admin-triggered first, not automatic background crawling.
* Store verification result safely.

Possible fields:

* `BadgeEnabled`
* `ReciprocalLinkVerified`
* `ReciprocalLinkVerifiedAt`
* `ReciprocalLinkLastCheckedAt`
* `ReciprocalLinkCheckError`

This is optional but recommended because it creates a stronger Developer Network identity.

---

# Public Profile Pages

Add individual profile pages if feasible:

* `/network/geric-morit`
* `/network/avery-macasa`
* `/network/john-darcy-atienza`

Each profile page may show:

* Name
* Role/title
* Portfolio URL
* GitHub URL
* Tags
* Connection type
* Short intro/message
* AI-assisted summary if approved
* Project highlights if approved
* Source links
* Open-to-collaborate badge
* Verified badge
* Featured badge
* Reciprocal link verified badge if applicable
* Last reviewed date
* Copyable badge snippet
* Back to Developer Network link

Do not expose:

* Email
* Internal notes
* Raw AI prompt
* Edit tokens
* Admin-only status details
* Private errors

---

# Suggested Data Model Additions

Inspect existing model first before deciding final schema.

Possible additive fields on existing collaboration entity:

* `Status`
* `IsVisible`
* `IsFeatured`
* `IsVerified`
* `OpenToCollaborate`
* `PublicSlug`
* `RoleTitle`
* `ConnectionType`
* `Tags`
* `GitHubUrl`
* `LinkedInUrl`
* `DisplayOrder`
* `AiEnrichmentConsent`
* `AiEnrichmentConsentAt`
* `LastAiEnrichmentAt`
* `EditTokenHash`
* `EditTokenExpiresAt`
* `EditTokenUsedAt`
* `LastEditRequestedAt`
* `LastEditedAt`
* `EmailVerifiedAt`
* `BadgeEnabled`
* `ReciprocalLinkVerified`
* `ReciprocalLinkVerifiedAt`
* `ReciprocalLinkLastCheckedAt`
* `ReciprocalLinkCheckError`
* `ReviewedAt`
* `ReviewedBy`
* `ReviewNote`
* `CreatedAt`
* `UpdatedAt`

Use only fields that are justified by the implementation.

Do not blindly add every field if a cleaner design exists.

Possible new tables:

1. `DeveloperNetworkProfileRevision`
2. `DeveloperProfileEnrichment`
3. `DeveloperNetworkAuditLog` if justified
4. `DeveloperNetworkEditToken` if better than token fields on main entity

---

# Migration Safety Requirements

Before applying any migration, report:

1. Existing entity/table name.
2. Existing fields.
3. Exact proposed fields.
4. Exact migration name.
5. Whether changes are additive.
6. How old records will be backfilled.
7. Whether any old data will be deleted.
8. Rollback risk.
9. Whether seed data changes are needed.
10. Whether admin pages need updates.

Migrations are allowed for Phase 3 only if:

* They are additive.
* They are directly related to Developer Network.
* They preserve old data.
* They do not change unrelated tables.
* They do not break existing collaboration submissions.

---

# Implementation Sub-Phases

Divide Phase 3 into sub-phases.

## Phase 3A: Current System Audit

Inspect and report:

* Collaboration model/entity
* Database table
* Existing fields
* Existing admin UI
* Existing public rendering
* Existing service methods
* Existing form submission flow
* Whether email currently exists
* Whether moderation currently exists
* Whether emails are publicly exposed
* Whether public profile pages exist
* Whether there are current migrations
* Where CSS for the section lives

No code changes in 3A except documentation/plan if necessary.

## Phase 3B: Data-Safe Foundation

Implement additive schema changes only after reporting exact fields.

Goals:

* Preserve old records.
* Add status/moderation fields.
* Add slug/public profile support.
* Add role/title/tags/connection type/open-to-collaborate.
* Add edit-token support.
* Add AI consent fields.
* Add timestamps/review fields.
* Add reciprocal link fields if included.

## Phase 3C: Public Developer Network UI

Goals:

* Rename/reframe section as Developer Network.
* Move section higher.
* Improve public cards.
* Add search.
* Add filters based on available data.
* Hide private information.
* Add badges.
* Add safe external links.
* Preserve Join My Network form.

## Phase 3D: Admin Moderation

Goals:

* Review pending requests.
* Approve/reject/hide.
* Feature/unfeature.
* Verify/unverify.
* Edit public metadata.
* Manage display order.
* View private email admin-only.
* See profile completeness.

## Phase 3E: Gmail SMTP Notifications

Goals:

* Add email service if not existing.
* Use environment variables.
* Send submission/admin/approval/rejection/edit-link/revision notifications.
* Add safe email templates.
* Do not log secrets.

## Phase 3F: Magic Edit Links and Pending Revisions

Goals:

* Request edit link by email.
* Send tokenized link.
* Edit profile through secure route.
* Save proposed changes as pending revision.
* Admin compares current vs proposed.
* Admin approves/rejects revision.

## Phase 3G: AI-Assisted Enrichment

Goals:

* Add Gemini service via environment variables.
* Respect consent.
* Generate structured JSON.
* Store draft enrichment separately.
* Store sources/citations.
* Admin reviews and approves.
* Public displays only approved enrichment.

## Phase 3H: Public Profile Pages

Goals:

* Add `/network/{slug}` pages.
* Show richer public information.
* Show sources for approved AI details.
* Show badge snippet if enabled.
* Do not expose private info.

## Phase 3I: Reciprocal Link Verification

Goals:

* Add optional badge snippet.
* Add admin-triggered verification.
* Check submitted portfolio URL only.
* Mark reciprocal link verified if link found.
* Do not crawl aggressively.

## Phase 3J: Abuse Protection and Final Build

Goals:

* Honeypot field.
* Rate limiting/cooldowns if feasible.
* Character limits.
* URL validation.
* `dotnet build`.
* Summarize changed files.
* List migrations.
* List unverified manual QA items.

---

# Required First Output

Before implementation, produce:

# Revised Phase 3 Implementation Plan: Developer Network Refactor

Include:

1. Current collaboration system overview.
2. Existing model/table fields.
3. Existing admin flow.
4. Existing public UI flow.
5. Existing privacy risks.
6. Proposed architecture.
7. Proposed schema changes.
8. Data preservation and backfill plan.
9. Gmail SMTP notification plan.
10. Magic edit link plan.
11. Pending revision plan.
12. AI enrichment plan.
13. Source/citation plan.
14. Reciprocal link verification plan.
15. Public UX layout.
16. Admin UX layout.
17. Files likely to change.
18. Migration plan.
19. Environment variables needed.
20. Security and privacy safeguards.
21. Abuse protection plan.
22. Verification plan.
23. Risks and rollback plan.
24. Sub-phase implementation order.

Do not implement until I approve the revised Phase 3 plan.

---

## Required Corrections (From Admin Review)

1. Before creating `Services/EmailService.cs`, inspect whether an email service already exists.
   - If it exists, extend it safely.
   - Do not create a duplicate email service.
   - Do not break existing contact form/email behavior.

2. Separate `IsVerified` from `ReciprocalLinkVerified`.
   - `IsVerified` means manually reviewed/verified by admin.
   - `ReciprocalLinkVerified` means the collaborator’s portfolio links back to `jhersonaguto.dev` or `/network`.
   - Use separate public badges:
     - Verified Profile
     - Reciprocal Link
     - Featured
     - Open to Collaborate

3. For LinkedIn or other professional links:
   - Only process public pages submitted by the user.
   - Do not bypass login, scraping restrictions, or private content.
   - If inaccessible, skip and store a warning.

4. Public visibility must require:
   - `Status == Approved`
   - `IsVisible == true`

5. Slug generation must handle old records safely.
   - For new records, generate `PublicSlug` on creation.
   - For old records, use a safe one-time backfill.
   - Handle duplicates by appending the record ID or another stable suffix.
   - Do not risk migration failure because of duplicate names.

6. AI enrichment must not overwrite submitted profile fields automatically.
   - Store AI output separately.
   - Admin can approve enrichment as a separate display layer.
   - Admin can optionally apply selected AI suggestions manually.

7. Add feature flags through configuration/environment variables:
   - `DEVELOPER_NETWORK_ENABLED`
   - `SMTP_NOTIFICATIONS_ENABLED`
   - `AI_ENRICHMENT_ENABLED`
   - `RECIPROCAL_LINK_CHECK_ENABLED`

8. Do not log:
   - SMTP app password
   - Gemini API key
   - raw magic tokens
   - private edit links
   - collaborator emails in public logs

9. Do not expose public emails.
   - Emails are admin-only.
   - Edit-link request response must be generic:
     “If this email exists in the Developer Network, an edit link has been sent.”

10. Build verification is required.
    - Run `dotnet build`.
    - Do not say build passed unless it actually passed.

---

## My verdict

Approve **after these corrections**.

The current plan is very good, but these revisions will make it safer, especially for:

- old collaboration data preservation
- email service compatibility
- AI output safety
- public privacy
- slug uniqueness
- deployment safety

Once Antigravity revises the plan, Phase 3B can start.
