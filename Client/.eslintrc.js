module.exports = {
  root: true,
  env: {
    browser: true,
    node: true
  },
  extends: [
    'plugin:vue/base',
    '@nuxtjs/eslint-config-typescript',
    'plugin:nuxt/recommended'
  ],
  plugins: [
    'vuetify'
  ],
  // add your custom rules here
  rules: {
    'vuetify/no-deprecated-classes': 'error'
  }
}
