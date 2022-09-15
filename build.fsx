#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open System
open Fake.Core
open Fake.IO
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators
open FileSystemOperators
open Fake.DotNet

let src = __SOURCE_DIRECTORY__ @@ "src"

let srcGlob = src @@ "**/*.??proj"
let testGlob = __SOURCE_DIRECTORY__  @@ "test/**/*.??proj"

let buildDir = "./build"

let clean _ =
    ["bin"; "temp" ; buildDir]
    |> Shell.cleanDirs

    !! srcGlob
    ++ testGlob
    |> Seq.collect(fun p ->
        ["bin";"obj"]
        |> Seq.map(fun sp ->
            let name = IO.Path.GetDirectoryName p @@ sp
            name)
        )
    |> Shell.cleanDirs

let dotnetBuild _ =
    !! srcGlob
      |> Seq.iter (DotNet.build  id)

let dotnetTest _ =
    !! testGlob
      |> Seq.iter (fun value ->
        DotNet.test id value)

Target.create "Clean" clean
Target.create "Build" dotnetBuild
Target.create "Test" dotnetTest


"Clean"
  ==> "Build"
  ==> "Test"

Target.runOrDefault "Test"
