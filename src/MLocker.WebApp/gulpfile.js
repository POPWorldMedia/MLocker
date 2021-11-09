/// <binding AfterBuild="default" />

var gulp = require("gulp"),
    merge = require("merge-stream");

var nodeRoot = "./node_modules/";
var targetPath = "./wwwroot/lib/";

gulp.task("copies", function () {
    var streams = [
        gulp.src(nodeRoot + "bootstrap/dist/**/*").pipe(gulp.dest(targetPath + "/bootstrap/dist")),
        gulp.src(nodeRoot + "@popperjs/core/dist/umd/**/*").pipe(gulp.dest(targetPath + "/popperjs/dist"))
    ];
    return merge(streams);
});

gulp.task("default", gulp.series("copies"));