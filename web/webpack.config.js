const Dotenv = require('dotenv-webpack');

module.exports = {
    entry: './src/index.js',
    output: {
        filename: 'main.js',
        libraryTarget: 'var',
        library: 'MemeOfTheYear'
    },
    mode: "production",
    plugins: [
        new Dotenv({systemvars: true})
    ]
};