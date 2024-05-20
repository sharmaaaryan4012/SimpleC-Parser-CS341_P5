namespace compiler

module parser =


  // this function extracts the head of the list compares and it with required token and returns the list 
  let private matchToken expected_token tokens =
    let next_token = List.head tokens                                                              // saving the head of the list into next_token
    if expected_token = next_token then  
      List.tail tokens
    else
      failwith ("expecting " + expected_token + ", but found " + next_token)                       // returning with failwith if the desired token is not found 


  // this function checks if the string matches with the literal by using the Startswith function 
  let beginswith (pattern: string) (literal: string) =        
    literal.StartsWith (pattern) 


  // this function matches the head of the token for identfier, int literals and string literals, if it matches it returns the tail of the list 
  let private matchIdentifier tokens =
    match tokens with
    | [] -> failwith "Unexpected end of input"
    | next_token :: rest when (beginswith "identifier:" next_token ) -> rest
    | next_token :: rest when (beginswith "int_literal:" next_token ) -> rest
    | next_token :: rest when (beginswith "real_literal:" next_token ) -> rest
    | next_token :: rest when (beginswith "str_literal:" next_token ) -> rest
    | next_token :: _ -> failwith ("expecting identifier, but found " + next_token)                  // returning with failwith explaining what was required 


  // this function checks the line of code for initialization line and compare each and every token for it  
  let  private vardecl tokens = 
    match tokens with
    | "int" :: _ -> 
                    let tokens2 = matchToken "int" tokens
                    let tokens3 = matchIdentifier tokens2                                                             // calls the matchIdentifier function for checking the Identifier 
                    matchToken ";" tokens3
                    
    | "real" :: _ -> 
                    let tokens2 = matchToken "real" tokens
                    let tokens3 = matchIdentifier tokens2                                                             // calls the matchIdentifier function for checking the Identifier 
                    matchToken ";" tokens3
    | _ -> tokens


  // this function checks the line of code for inputting something from the user
  let private input_func tokens = 
    let tokens2 = matchToken "cin" tokens
    let tokens3 = matchToken ">>" tokens2                                                             // compares the keyword ">>" with the code line token by using the function matchToken
    let tokens4 = matchIdentifier tokens3
    matchToken ";" tokens4


  // this function checks the expression line for identifier, int_literal and str_literal and others by using the appropriate function 
  let private exprValue tokens =
    match tokens with
    | hd :: _ when (beginswith "identifier:" hd) -> matchIdentifier tokens
    | hd :: _ when (beginswith "int_literal:" hd) -> matchIdentifier tokens
    | hd :: _ when (beginswith "real_literal:" hd) -> matchIdentifier tokens
    | hd :: _ when (beginswith "str_literal:" hd) -> matchIdentifier tokens
    | "true" :: _ -> matchToken "true" tokens                                                               // compares the token with true keeyword if matches returns the list 
    | "false" :: _ -> matchToken "false" tokens
    | next_token :: _ -> failwith ("expecting identifier or literal, but found " + next_token)
    | _ -> tokens


  // this function compares the token with the different types of keyword
  let private exprOp tokens =
    match tokens with
    | "+" :: _ | "-" :: _ | "*" :: _ | "/" :: _ | "^" :: _ | "<" :: _ | "<=" :: _ | ">" :: _ | ">=" :: _ | "==" :: _ | "!=" :: _ -> 
        let tokens = matchToken (List.head tokens) tokens
        exprValue tokens
    | _ -> tokens


  // this function takes the expression and calls the functions exprValue and then exprOp by passing the tail of the list 
  let private expr tokens =
    let tokens = exprValue tokens
    exprOp tokens


  // this function checks the token for endl, if not found it calls the expr function for the desired token check
  let private outputValue tokens =
    match tokens with
    | "endl" :: _ -> matchToken "endl" tokens
    | _ -> expr tokens


  // this function checks the line for outputting it checks each token by calling matchToken and then passes the tail to a different token check 
  let private output tokens = 
    let tokens2 = matchToken "cout" tokens
    let tokens3 = matchToken "<<" tokens2
    let tokens4 = outputValue tokens3
    matchToken ";" tokens4
  

  // the function checks for assignment line, again it checkseach token by match token and passes the tail to the different token check call
  let private assignment tokens = 
    let tokens2 = matchIdentifier tokens
    let tokens3 = matchToken "=" tokens2
    let tokens4 = expr tokens3
    matchToken ";" tokens4
    

  // thsi function defines which line of code we are checking by matching the first token of the line and then calls the desired function 
  let rec private stmt tokens = 
      match tokens with 
      | ";" :: tl -> matchToken ";" tokens
      | "int" :: tl ->  vardecl tokens
      | "real" :: tl ->  vardecl tokens
      | "cin" :: tl -> input_func tokens
      | "cout" :: tl -> output tokens                                                 // checks if its a cout line then calls the function which chekcs for the cout line
      | "if" :: tl -> ifstmt tokens
      | hd :: _ when beginswith "identifier:" hd -> assignment tokens
      | next_token :: _ -> failwith ("expecting statement, but found " + next_token) 
      |  _ -> tokens
      // if nothing matches then it calls the failwith function and shows the error

  // using the and operator instead of let to do mutual recursive calls
  and private then_part tokens = 
    stmt tokens

  // this function checks for the else part of the if statement 
  and private else_part tokens =
    let next_token = List.head tokens

    if next_token = "else" then
      let T2 = matchToken "else" tokens                                               // checks the else keyword, if matches then returns the tail
      stmt T2 
    else
      tokens 


  // this function checks for each token for the if line by calling the desired function 
  and ifstmt tokens = 
    let tokens2 = matchToken "if" tokens
    let tokens3 = matchToken "(" tokens2
    let tokens4 = expr tokens3                                        // calls the expr function and checks the whole expression in the if statement
    let tokens5 = matchToken ")" tokens4
    let tokens6 = then_part tokens5
    let tokens7 = else_part tokens6
    tokens7                                                           // calling the tokens7 function which automatically calls every small function


  // this function is called for the other statement which also checks for the end of statement 
  let rec private morestmts tokens =
    match tokens with
    | "}" :: _ -> tokens                                                // case of the statements are finished 
    | _ -> 
        let tokensAfterStmt = stmt tokens
        morestmts tokensAfterStmt


  // this functions calls for each line and passes the other code in morestmts function which agains calls stmt function
  let rec private stmts tokens =
    let rest_list = stmt tokens
    let rest_list2 = morestmts rest_list
    rest_list2

  // this function is checking the basic outline like void function, parenthesis and curly braces
  let private simpleC tokens = 

      let T2 = matchToken "void" tokens
      let T3 = matchToken "main" T2
      let T4 = matchToken "(" T3
      let T5 = matchToken ")" T4
      let T6 = matchToken "{" T5
      let T7 = stmts T6
      let T8 = matchToken "}" T7
      let T9 = matchToken "$" T8 // $ => EOF, there should be no more tokens
      T9

  // this function uses the try and catch function which either displays success or the error
  let parse tokens = 
    try
      let result = simpleC tokens
      "Success!"
    with 
      | ex -> "syntax_error: " + ex.Message