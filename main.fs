(*
Name: Aaryan Sharma
Program - 5 : SimpleC Type Checking
CS - 341 (Spring 2024)
Professor: Ellen Kidane
*)


[<EntryPoint>]
let main argv =
  //
  printf "SimpleC filename> "
  let filename = System.Console.ReadLine()
  printfn ""
  //
  if not (System.IO.File.Exists(filename)) then
    printfn "**Error: file '%s' does not exist." filename
    0
  else
    printfn "Compiling %s..." filename
    //
    // Run the lexer to get the tokens, and then
    // pass these tokens to the parser to see if
    // the input program is legal:
    //
    let tokens = compiler.lexer.analyze filename
    //
    printfn ""
    printfn "%A" tokens
    printfn ""
    //
    printfn "Parsing %s..." filename
    let result = compiler.parser.parse tokens
    printfn "%s" result
    printfn ""
    //
    if result <> "Success!" then
      exit(0)
    //
    // no syntax errors, perform analysis
    //
    printfn "Analyzing %s..." filename
    let (result, symboltable) = compiler.analyzer.build_symboltable tokens
    printfn "%s" result
    printfn ""
    //
    if result <> "success" then
      exit(0)
    //
    // symbol table built, now type-checking...
    //
    printfn "Symbol table: %A" symboltable
    printfn ""
    //
    printfn "Type-checking %s..." filename
    let result = compiler.checker.typecheck tokens symboltable
    printfn "%s" result
    printfn ""
    //
    0
