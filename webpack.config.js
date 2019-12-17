const path = require('path');
const nodeExternals = require('webpack-node-externals');

const isProduction = process.env['NODE_ENV'] === 'production';
const mode = isProduction ? 'production' : 'development';

module.exports = {
  mode,
  entry: './src/Tusk.fsproj',
  target: 'node',
  devtool: isProduction ? false : 'source-map',
  output: {
    path: path.join(__dirname, './lib'),
    filename: 'index.js',
  },
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: 'fable-loader',
      },
    ],
  },
  resolve: {
    alias: {
      'pg-native': path.join(__dirname, 'aliases/pg-native.js'),
    },
  },
  externals: [
    nodeExternals(),
  ],
};
