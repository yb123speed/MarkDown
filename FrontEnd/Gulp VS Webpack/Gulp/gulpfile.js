const gulp = require('gulp');
const less = require('gulp-less');

gulp.task('build:less', function(){
    return gulp.src('./src/*.less')
        .pipe(less())
        .pipe(gulp.dest('./dist'));
});