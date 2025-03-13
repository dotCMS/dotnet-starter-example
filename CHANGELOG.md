# Changelog

All notable changes to the dotCMS .NET SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Proxy feature to forward specific requests to dotCMS or other target servers
  - Added `ProxyConfig` model to represent proxy configuration from appsettings.json
  - Added `ProxyActionFilter` to handle the proxying logic
  - Added `ProxyAttribute` to apply the filter to controllers
  - Added documentation for the proxy feature in `docs/Proxy.md`
  - Configured named HttpClient for proxy requests
  - Updated README.md to document the proxy feature
  - Removed proxy-related TODO item as it's now implemented

### Changed
- Applied `ProxyAttribute` to `DotCmsUVEController` to enable proxying for all requests
- Updated Program.cs to register the ProxyActionFilter and configure the HttpClient

### Fixed
- Fixed issue with assets and content not being properly served from dotCMS by implementing the proxy feature
