# Security Policy

Prism Software LLC takes the security of the Prism Library seriously. This
document describes how to report a vulnerability and how we handle security
issues.

## Supported versions

Security fixes are provided for the **current major/minor release line** of
Prism. Users on older lines are encouraged to upgrade to a supported release to
receive security updates. Support obligations for Commercial and Commercial Plus
subscribers are defined by their license agreement.

## Reporting a vulnerability

**Please do not report security vulnerabilities through public GitHub issues,
discussions, or pull requests.**

Report vulnerabilities privately using **GitHub's private vulnerability
reporting** for this repository:

1. Go to the **Security** tab of the [PrismLibrary/Prism](https://github.com/PrismLibrary/Prism) repository.
2. Click **Report a vulnerability** to open a private security advisory.

Alternatively, you may email **support@prismlibrary.com** with the subject line
`SECURITY` and we will coordinate a private channel.

Please include, to the extent possible:

- A description of the vulnerability and its potential impact
- The affected Prism package(s) and version(s)
- Steps to reproduce, proof-of-concept, or relevant code
- Any suggested mitigation

## What to expect

- **Acknowledgement:** we aim to acknowledge your report within **5 business days**.
- **Assessment:** we will investigate, validate, and assess severity (using CVSS)
  and keep you informed of progress.
- **Remediation:** confirmed vulnerabilities are addressed on a risk-based
  schedule — higher-severity issues are prioritized — and fixes are delivered in
  a patch release.
- **Disclosure:** we follow **coordinated disclosure**. We will work with you on
  timing and, unless you prefer otherwise, credit you in the published advisory
  once a fix is available.

## Scope

Prism is a client-side application **framework** that is compiled into
applications built by its users; it does not host or process end-user data and
operates no production service. Reports should concern vulnerabilities in the
Prism source code or its distributed NuGet packages. Issues in **applications
built with Prism** should be reported to the maintainers of those applications.

## Package integrity

Prism packages are distributed via NuGet.org, are **author-signed** by Prism and
repository-signed by NuGet.org, and carry content hashes. Consumers can verify
package signatures and hashes on restore to confirm authenticity and integrity.

Thank you for helping keep Prism and its users safe.
