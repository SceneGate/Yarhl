// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require('prism-react-renderer/themes/github');
const darkCodeTheme = require('prism-react-renderer/themes/dracula');

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'Yarhl',
  tagline: 'A format ResearcH Library',
  favicon: 'img/favicon.ico',

  // Set the production url of your site here
  url: 'https://scenegate.github.io',
  // Set the /<baseUrl>/ pathname under which your site is served
  // For GitHub pages deployment, it is often '/<projectName>/'
  baseUrl: '/Yarhl',

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: 'SceneGate', // Usually your GitHub org/user name.
  projectName: 'Yarhl', // Usually your repo name.

  onBrokenLinks: 'warn',
  onBrokenMarkdownLinks: 'warn',

  // Even if you don't use internalization, you can use this field to set useful
  // metadata like html lang. For example, if your site is Chinese, you may want
  // to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          routeBasePath: '/',
          sidebarPath: require.resolve('./sidebars.js'),
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl:
            'https://github.com/SceneGate/Yarhl/tree/develop/',
        },
        blog: false,
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
      }),
    ],
  ],
  themes: ['@docusaurus/theme-mermaid'],
  markdown: {
    mermaid: true,
  },
  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      // Replace with your project's social card
      image: 'img/logo-large.png',
      navbar: {
        title: 'Yarhl',
        logo: {
          alt: 'Yarhl',
          src: 'img/mister.png',
        },
        items: [
          {
            type: 'docSidebar',
            sidebarId: 'mainDocsSidebar',
            position: 'left',
            label: 'Guides',
          },
          {
            type: 'docSidebar',
            sidebarId: 'apiDocsSidebar',
            position: 'left',
            label: 'API',
          },
         // {to: '/docs/api', label: 'API', position: 'left'},
          {
            href: 'https://github.com/SceneGate/Yarhl',
            className: 'header-github-link',
            'aria-label': 'GitHub repository',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'Site',
            items: [
              { label: 'Documentation', to: '/docs/intro' },
              { label: 'API', to: '/docs/api' },
            ],
          },
          {
            title: 'Community',
            items: [
              {
                label: '@pleonex',
                href: 'https://fosstodon.org/@pleonex',
              },
              {
                label: 'TraduSquare',
                href: 'https://tradusquare.es',
              },
            ],
          },
          {
            title: 'More',
            items: [
              {
                label: 'GitHub',
                href: 'https://github.com/SceneGate/Yarhl',
              },
              {
                label: 'SceneGate framework',
                href: 'https://github.com/SceneGate',
              },
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} SceneGate. Built with Docusaurus.`,
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
        additionalLanguages: ['csharp'],
      },
    }),
};

module.exports = config;
