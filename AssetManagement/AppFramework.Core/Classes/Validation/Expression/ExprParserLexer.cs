// $ANTLR 3.1.2 C:\\Temp\\antlr\\ExprParser.g 2009-05-01 15:10:33

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  AppFramework.Core.Classes.Validation.Expression 
{

using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;


public partial class ExprParserLexer : Lexer {
    public const int WS = 10;
    public const int T__16 = 16;
    public const int T__15 = 15;
    public const int NEWLINE = 4;
    public const int T__12 = 12;
    public const int T__11 = 11;
    public const int T__14 = 14;
    public const int T__13 = 13;
    public const int BOOL = 6;
    public const int ALIAS = 7;
    public const int INT = 9;
    public const int NOT = 8;
    public const int ID = 5;
    public const int EOF = -1;

    // delegates
    // delegators

    public ExprParserLexer() 
    {
		InitializeCyclicDFAs();
    }
    public ExprParserLexer(ICharStream input)
		: this(input, null) {
    }
    public ExprParserLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state) {
		InitializeCyclicDFAs(); 

    }
    
    override public string GrammarFileName
    {
    	get { return "C:\\Temp\\antlr\\ExprParser.g";} 
    }

    // $ANTLR start "T__11"
    public void mT__11() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__11;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:9:7: ( '=' )
            // C:\\Temp\\antlr\\ExprParser.g:9:9: '='
            {
            	Match('='); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__11"

    // $ANTLR start "T__12"
    public void mT__12() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__12;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:10:7: ( ' OR ' )
            // C:\\Temp\\antlr\\ExprParser.g:10:9: ' OR '
            {
            	Match(" OR "); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__12"

    // $ANTLR start "T__13"
    public void mT__13() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__13;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:11:7: ( 'NOT ' )
            // C:\\Temp\\antlr\\ExprParser.g:11:9: 'NOT '
            {
            	Match("NOT "); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__13"

    // $ANTLR start "T__14"
    public void mT__14() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__14;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:12:7: ( ' AND ' )
            // C:\\Temp\\antlr\\ExprParser.g:12:9: ' AND '
            {
            	Match(" AND "); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__14"

    // $ANTLR start "T__15"
    public void mT__15() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__15;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:13:7: ( '(' )
            // C:\\Temp\\antlr\\ExprParser.g:13:9: '('
            {
            	Match('('); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__15"

    // $ANTLR start "T__16"
    public void mT__16() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__16;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:14:7: ( ')' )
            // C:\\Temp\\antlr\\ExprParser.g:14:9: ')'
            {
            	Match(')'); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__16"

    // $ANTLR start "BOOL"
    public void mBOOL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BOOL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:69:6: ( 'true' | 'false' )
            int alt1 = 2;
            int LA1_0 = input.LA(1);

            if ( (LA1_0 == 't') )
            {
                alt1 = 1;
            }
            else if ( (LA1_0 == 'f') )
            {
                alt1 = 2;
            }
            else 
            {
                NoViableAltException nvae_d1s0 =
                    new NoViableAltException("", 1, 0, input);

                throw nvae_d1s0;
            }
            switch (alt1) 
            {
                case 1 :
                    // C:\\Temp\\antlr\\ExprParser.g:69:9: 'true'
                    {
                    	Match("true"); 


                    }
                    break;
                case 2 :
                    // C:\\Temp\\antlr\\ExprParser.g:69:16: 'false'
                    {
                    	Match("false"); 


                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BOOL"

    // $ANTLR start "NOT"
    public void mNOT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NOT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:70:7: ( 'NOT' )
            // C:\\Temp\\antlr\\ExprParser.g:70:9: 'NOT'
            {
            	Match("NOT"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NOT"

    // $ANTLR start "ID"
    public void mID() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ID;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:71:5: ( ( 'a' .. 'z' | 'A' .. 'Z' )+ )
            // C:\\Temp\\antlr\\ExprParser.g:71:9: ( 'a' .. 'z' | 'A' .. 'Z' )+
            {
            	// C:\\Temp\\antlr\\ExprParser.g:71:9: ( 'a' .. 'z' | 'A' .. 'Z' )+
            	int cnt2 = 0;
            	do 
            	{
            	    int alt2 = 2;
            	    int LA2_0 = input.LA(1);

            	    if ( ((LA2_0 >= 'A' && LA2_0 <= 'Z') || (LA2_0 >= 'a' && LA2_0 <= 'z')) )
            	    {
            	        alt2 = 1;
            	    }


            	    switch (alt2) 
            		{
            			case 1 :
            			    // C:\\Temp\\antlr\\ExprParser.g:
            			    {
            			    	if ( (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            			    	{
            			    	    input.Consume();

            			    	}
            			    	else 
            			    	{
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    if ( cnt2 >= 1 ) goto loop2;
            		            EarlyExitException eee2 =
            		                new EarlyExitException(2, input);
            		            throw eee2;
            	    }
            	    cnt2++;
            	} while (true);

            	loop2:
            		;	// Stops C# compiler whinging that label 'loop2' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ID"

    // $ANTLR start "INT"
    public void mINT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:72:5: ( ( '0' .. '9' )+ )
            // C:\\Temp\\antlr\\ExprParser.g:72:9: ( '0' .. '9' )+
            {
            	// C:\\Temp\\antlr\\ExprParser.g:72:9: ( '0' .. '9' )+
            	int cnt3 = 0;
            	do 
            	{
            	    int alt3 = 2;
            	    int LA3_0 = input.LA(1);

            	    if ( ((LA3_0 >= '0' && LA3_0 <= '9')) )
            	    {
            	        alt3 = 1;
            	    }


            	    switch (alt3) 
            		{
            			case 1 :
            			    // C:\\Temp\\antlr\\ExprParser.g:72:9: '0' .. '9'
            			    {
            			    	MatchRange('0','9'); 

            			    }
            			    break;

            			default:
            			    if ( cnt3 >= 1 ) goto loop3;
            		            EarlyExitException eee3 =
            		                new EarlyExitException(3, input);
            		            throw eee3;
            	    }
            	    cnt3++;
            	} while (true);

            	loop3:
            		;	// Stops C# compiler whinging that label 'loop3' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INT"

    // $ANTLR start "ALIAS"
    public void mALIAS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ALIAS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:73:7: ( '@' ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' )+ )
            // C:\\Temp\\antlr\\ExprParser.g:73:9: '@' ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' )+
            {
            	Match('@'); 
            	// C:\\Temp\\antlr\\ExprParser.g:73:12: ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' )+
            	int cnt4 = 0;
            	do 
            	{
            	    int alt4 = 2;
            	    int LA4_0 = input.LA(1);

            	    if ( ((LA4_0 >= '0' && LA4_0 <= '9') || (LA4_0 >= 'A' && LA4_0 <= 'Z') || (LA4_0 >= 'a' && LA4_0 <= 'z')) )
            	    {
            	        alt4 = 1;
            	    }


            	    switch (alt4) 
            		{
            			case 1 :
            			    // C:\\Temp\\antlr\\ExprParser.g:
            			    {
            			    	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            			    	{
            			    	    input.Consume();

            			    	}
            			    	else 
            			    	{
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    if ( cnt4 >= 1 ) goto loop4;
            		            EarlyExitException eee4 =
            		                new EarlyExitException(4, input);
            		            throw eee4;
            	    }
            	    cnt4++;
            	} while (true);

            	loop4:
            		;	// Stops C# compiler whinging that label 'loop4' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ALIAS"

    // $ANTLR start "NEWLINE"
    public void mNEWLINE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NEWLINE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:74:8: ( ( '\\r' )? '\\n' )
            // C:\\Temp\\antlr\\ExprParser.g:74:9: ( '\\r' )? '\\n'
            {
            	// C:\\Temp\\antlr\\ExprParser.g:74:9: ( '\\r' )?
            	int alt5 = 2;
            	int LA5_0 = input.LA(1);

            	if ( (LA5_0 == '\r') )
            	{
            	    alt5 = 1;
            	}
            	switch (alt5) 
            	{
            	    case 1 :
            	        // C:\\Temp\\antlr\\ExprParser.g:74:9: '\\r'
            	        {
            	        	Match('\r'); 

            	        }
            	        break;

            	}

            	Match('\n'); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NEWLINE"

    // $ANTLR start "WS"
    public void mWS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // C:\\Temp\\antlr\\ExprParser.g:75:5: ( ( ' ' | '\\t' )+ )
            // C:\\Temp\\antlr\\ExprParser.g:75:9: ( ' ' | '\\t' )+
            {
            	// C:\\Temp\\antlr\\ExprParser.g:75:9: ( ' ' | '\\t' )+
            	int cnt6 = 0;
            	do 
            	{
            	    int alt6 = 2;
            	    int LA6_0 = input.LA(1);

            	    if ( (LA6_0 == '\t' || LA6_0 == ' ') )
            	    {
            	        alt6 = 1;
            	    }


            	    switch (alt6) 
            		{
            			case 1 :
            			    // C:\\Temp\\antlr\\ExprParser.g:
            			    {
            			    	if ( input.LA(1) == '\t' || input.LA(1) == ' ' ) 
            			    	{
            			    	    input.Consume();

            			    	}
            			    	else 
            			    	{
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    if ( cnt6 >= 1 ) goto loop6;
            		            EarlyExitException eee6 =
            		                new EarlyExitException(6, input);
            		            throw eee6;
            	    }
            	    cnt6++;
            	} while (true);

            	loop6:
            		;	// Stops C# compiler whinging that label 'loop6' has no statements


            			//skip();
            		

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WS"

    override public void mTokens() // throws RecognitionException 
    {
        // C:\\Temp\\antlr\\ExprParser.g:1:8: ( T__11 | T__12 | T__13 | T__14 | T__15 | T__16 | BOOL | NOT | ID | INT | ALIAS | NEWLINE | WS )
        int alt7 = 13;
        alt7 = dfa7.Predict(input);
        switch (alt7) 
        {
            case 1 :
                // C:\\Temp\\antlr\\ExprParser.g:1:10: T__11
                {
                	mT__11(); 

                }
                break;
            case 2 :
                // C:\\Temp\\antlr\\ExprParser.g:1:16: T__12
                {
                	mT__12(); 

                }
                break;
            case 3 :
                // C:\\Temp\\antlr\\ExprParser.g:1:22: T__13
                {
                	mT__13(); 

                }
                break;
            case 4 :
                // C:\\Temp\\antlr\\ExprParser.g:1:28: T__14
                {
                	mT__14(); 

                }
                break;
            case 5 :
                // C:\\Temp\\antlr\\ExprParser.g:1:34: T__15
                {
                	mT__15(); 

                }
                break;
            case 6 :
                // C:\\Temp\\antlr\\ExprParser.g:1:40: T__16
                {
                	mT__16(); 

                }
                break;
            case 7 :
                // C:\\Temp\\antlr\\ExprParser.g:1:46: BOOL
                {
                	mBOOL(); 

                }
                break;
            case 8 :
                // C:\\Temp\\antlr\\ExprParser.g:1:51: NOT
                {
                	mNOT(); 

                }
                break;
            case 9 :
                // C:\\Temp\\antlr\\ExprParser.g:1:55: ID
                {
                	mID(); 

                }
                break;
            case 10 :
                // C:\\Temp\\antlr\\ExprParser.g:1:58: INT
                {
                	mINT(); 

                }
                break;
            case 11 :
                // C:\\Temp\\antlr\\ExprParser.g:1:62: ALIAS
                {
                	mALIAS(); 

                }
                break;
            case 12 :
                // C:\\Temp\\antlr\\ExprParser.g:1:68: NEWLINE
                {
                	mNEWLINE(); 

                }
                break;
            case 13 :
                // C:\\Temp\\antlr\\ExprParser.g:1:76: WS
                {
                	mWS(); 

                }
                break;

        }

    }


    protected DFA7 dfa7;
	private void InitializeCyclicDFAs()
	{
	    this.dfa7 = new DFA7(this);
	}

    const string DFA7_eotS =
        "\x02\uffff\x01\x0c\x01\x08\x02\uffff\x02\x08\x07\uffff\x03\x08"+
        "\x01\x16\x02\x08\x02\uffff\x01\x19\x01\x08\x01\uffff\x01\x19";
    const string DFA7_eofS =
        "\x1b\uffff";
    const string DFA7_minS =
        "\x01\x09\x01\uffff\x01\x41\x01\x4f\x02\uffff\x01\x72\x01\x61\x07"+
        "\uffff\x01\x54\x01\x75\x01\x6c\x01\x20\x01\x65\x01\x73\x02\uffff"+
        "\x01\x41\x01\x65\x01\uffff\x01\x41";
    const string DFA7_maxS =
        "\x01\x7a\x01\uffff\x02\x4f\x02\uffff\x01\x72\x01\x61\x07\uffff"+
        "\x01\x54\x01\x75\x01\x6c\x01\x7a\x01\x65\x01\x73\x02\uffff\x01\x7a"+
        "\x01\x65\x01\uffff\x01\x7a";
    const string DFA7_acceptS =
        "\x01\uffff\x01\x01\x02\uffff\x01\x05\x01\x06\x02\uffff\x01\x09"+
        "\x01\x0a\x01\x0b\x01\x0c\x01\x0d\x01\x02\x01\x04\x06\uffff\x01\x03"+
        "\x01\x08\x02\uffff\x01\x07\x01\uffff";
    const string DFA7_specialS =
        "\x1b\uffff}>";
    static readonly string[] DFA7_transitionS = {
            "\x01\x0c\x01\x0b\x02\uffff\x01\x0b\x12\uffff\x01\x02\x07\uffff"+
            "\x01\x04\x01\x05\x06\uffff\x0a\x09\x03\uffff\x01\x01\x02\uffff"+
            "\x01\x0a\x0d\x08\x01\x03\x0c\x08\x06\uffff\x05\x08\x01\x07\x0d"+
            "\x08\x01\x06\x06\x08",
            "",
            "\x01\x0e\x0d\uffff\x01\x0d",
            "\x01\x0f",
            "",
            "",
            "\x01\x10",
            "\x01\x11",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\x12",
            "\x01\x13",
            "\x01\x14",
            "\x01\x15\x20\uffff\x1a\x08\x06\uffff\x1a\x08",
            "\x01\x17",
            "\x01\x18",
            "",
            "",
            "\x1a\x08\x06\uffff\x1a\x08",
            "\x01\x1a",
            "",
            "\x1a\x08\x06\uffff\x1a\x08"
    };

    static readonly short[] DFA7_eot = DFA.UnpackEncodedString(DFA7_eotS);
    static readonly short[] DFA7_eof = DFA.UnpackEncodedString(DFA7_eofS);
    static readonly char[] DFA7_min = DFA.UnpackEncodedStringToUnsignedChars(DFA7_minS);
    static readonly char[] DFA7_max = DFA.UnpackEncodedStringToUnsignedChars(DFA7_maxS);
    static readonly short[] DFA7_accept = DFA.UnpackEncodedString(DFA7_acceptS);
    static readonly short[] DFA7_special = DFA.UnpackEncodedString(DFA7_specialS);
    static readonly short[][] DFA7_transition = DFA.UnpackEncodedStringArray(DFA7_transitionS);

    protected class DFA7 : DFA
    {
        public DFA7(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 7;
            this.eot = DFA7_eot;
            this.eof = DFA7_eof;
            this.min = DFA7_min;
            this.max = DFA7_max;
            this.accept = DFA7_accept;
            this.special = DFA7_special;
            this.transition = DFA7_transition;

        }

        override public string Description
        {
            get { return "1:1: Tokens : ( T__11 | T__12 | T__13 | T__14 | T__15 | T__16 | BOOL | NOT | ID | INT | ALIAS | NEWLINE | WS );"; }
        }

    }

 
    
}
}