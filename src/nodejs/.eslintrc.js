module.exports = {
  env: {
    browser: true,
    commonjs: true,
    es2021: true,
  },
  extends: ['airbnb-base', 'plugin:sonarjs/recommended'],
  overrides: [],
  parserOptions: {
    ecmaVersion: 'latest',
  },
  plugins: ['sonarjs'],
  rules: {
    'linebreak-style': 0,
    'max-len': 0,
    'class-methods-use-this': 0,
  },
};
