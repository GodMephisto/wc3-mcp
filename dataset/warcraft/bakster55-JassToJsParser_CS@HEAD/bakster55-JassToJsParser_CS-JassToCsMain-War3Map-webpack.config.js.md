---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/War3Map/webpack.config.js"
ref: HEAD
---

var path = require('path');
var webpack = require('webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');

module.exports = {
	entry: {
		app: './app.js',
	},
	mode: 'development',
	devtool: 'cheap-module-source-map',
	output: {
		path: path.join(__dirname, 'dist'),
		filename: '[name].js',
	},
	optimization: {
		runtimeChunk: "single",
		splitChunks: {
			chunks: 'all',
		}
	},
	plugins: [
		new webpack.DefinePlugin({
			"__dirname": JSON.stringify(__dirname),
		}),
		new HtmlWebpackPlugin(),
		new CleanWebpackPlugin(),
		new webpack.LoaderOptionsPlugin({
		  debug: true
		})
	]
};