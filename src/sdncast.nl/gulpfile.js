/// <binding AfterBuild='default' Clean='clean' />

"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    csslint = require('gulp-csslint'),
    cssmin = require("gulp-cssmin"),
    jshint = require('gulp-jshint'),
    uglify = require("gulp-uglify"),
    rimraf = require("rimraf"),
    path = require("path");

var webroot = "./wwwroot/"; 

var library = {
    base: "node_modules",
    destination: "lib",
    source: [
        // glob pattern to get the dirname and match only js and min.js file wanted
        path.dirname(require.resolve('jquery-validation-unobtrusive/jquery.validate.unobtrusive.js')) + '/*unobtrusive**.js',
        // alternative of declaring each file
        require.resolve('bootstrap/dist/js/bootstrap.js'),
        require.resolve('bootstrap/dist/js/bootstrap.min.js'),
        require.resolve('bootstrap/dist/css/bootstrap.css'),
        require.resolve('bootstrap/dist/css/bootstrap.min.css'),
        // glob pattern to get all files within the directory
        path.dirname(require.resolve('bootstrap/dist/fonts/glyphicons-halflings-regular.woff')) + '/**',
        // declare each file
        require.resolve('jquery/dist/jquery.js'),
        require.resolve('jquery/dist/jquery.min.js'),
        // only one file is distributed
        require.resolve('jquery-validation/dist/jquery.validate.js')
    ]
};

var paths = {
    library: webroot + library.destination,
    js: webroot + "js/**/*.js",
    minJs: webroot + "js/**/*.min.js",
    css: webroot + "css/**/*.css",
    minCss: webroot + "css/**/*.min.css",
    concatJsDest: webroot + "js/site.min.js",
    concatCssDest: webroot + "css/site.min.css"
};

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean:lib", function (cb) {
    rimraf(paths.library, cb);
});

gulp.task("clean", ["clean:js", "clean:css", "clean:lib"]);

gulp.task("lib", ["clean"], function () {
  return gulp.src(library.source, { base: library.base })
    .pipe(gulp.dest(paths.library));
});

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["lib", "csslint", "jshint", "min:js", "min:css"]);

gulp.task("jshint", ["lib"], function(cb) {
    return gulp.src([paths.js, "!" + paths.minJs])
        .pipe(jshint())
        .pipe(jshint.reporter(), cb);
});

gulp.task("csslint", ["lib"], function() {
    return gulp.src([paths.css, "!"  + paths.minCss])
        .pipe(csslint())
        .pipe(csslint.formatter());
});

gulp.task("prepublish", ["lib", "csslint", "jshint",  "min"]);

gulp.task("default", ["prepublish"]);
