const path = require('path');
const del = require('del');
const gulp = require('gulp');
const npmdist = require('gulp-npm-dist');
const rename = require('gulp-rename');
const sass = require('gulp-sass')(require('sass'));
const autoprefixer = require("gulp-autoprefixer");
const sourcemaps = require("gulp-sourcemaps");    
const cleanCSS = require('gulp-clean-css');

const paths = {
    base: {
        base: {
            dir: './'
        },
        node: {
            dir: './node_modules'
        }
    },
    src: {
        base: {
            dir: './wwwroot/',
            files: './wwwroot/**/*'
        },
        libs: {
            dir: './wwwroot/assets/libs'
        },
        css: {
            dir: './wwwroot/assets/css',
            files: './wwwroot/assets/css/**/*'
        },
        scss: {
            dir: './wwwroot/assets/scss',
            files: './wwwroot/assets/scss/**/*',
            main: [
                './wwwroot/assets/scss/config/material/bootstrap.scss',
                './wwwroot/assets/scss/config/material/app.scss',
                './wwwroot/assets/scss/config/material/custom.scss',
                './wwwroot/assets/scss/icons.scss'
            ]
        }
    }
};


gulp.task('watch', function () {
  gulp.watch(paths.src.scss.files, gulp.series('scss'));
});


gulp.task('scss', function () {
  return gulp
      .src(paths.src.scss.main)
      .pipe(sourcemaps.init())
      .pipe(sass({
          includePaths: [path.resolve('./')]
      }).on('error', sass.logError))
      .pipe(
          autoprefixer()
      )
      .pipe(gulp.dest(paths.src.css.dir))
      .pipe(cleanCSS())
      .pipe(
          rename({
              suffix: ".min"
          })
      )
      .pipe(sourcemaps.write("./"))
      .pipe(gulp.dest(paths.src.css.dir));
});

gulp.task('copy:libs', function () {
  return gulp
      .src(npmdist(), { base: paths.base.node.dir })
      .pipe(rename(function (path) {
          path.dirname = path.dirname.replace(/\/dist/, '').replace(/\\dist/, '');
      }))
      .pipe(gulp.dest(paths.src.libs.dir));
});

gulp.task('clean:velzon', function (callback) {
  del.sync(paths.src.libs.dir);
  callback();
});

// Write a build stamp so MSBuild incremental targets can detect up-to-date outputs
gulp.task('stamp', function (callback) {
  const fs = require('fs');
  fs.writeFileSync('./wwwroot/assets/css/.build-stamp', new Date().toISOString());
  callback();
});

gulp.task('build', gulp.series(gulp.parallel('clean:velzon', 'copy:libs'), 'scss', 'stamp'));
gulp.task('default', gulp.series(gulp.parallel('clean:velzon', 'copy:libs', 'scss'), 'stamp', gulp.parallel('watch')));
