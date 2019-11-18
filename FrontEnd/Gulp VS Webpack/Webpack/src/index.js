const path = require('path');

module.exports = {
    entry: './src/index.js',        // 告诉 webpack 你要编译哪个文件
    output: {                       // 告诉 webpack 你要把编译后生成的文件放在哪
        filename: 'bundle.js',
        path: path.join(__dirname, 'dist')
    }
};