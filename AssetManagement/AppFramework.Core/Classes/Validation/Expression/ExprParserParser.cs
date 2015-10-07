// $ANTLR 3.1.2 C:\\Temp\\antlr\\ExprParser.g 2009-05-01 15:10:33

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  AppFramework.Core.Classes.Validation.Expression 
{

using System.Collections;
using System.Collections.Generic;

using AppFramework.Core.Classes.Validation;
using AppFramework.Core.Classes.Validation.Expression;


using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;


public partial class ExprParserParser : Parser
{
    public static readonly string[] tokenNames = new string[] 
	{
        "<invalid>", 
		"<EOR>", 
		"<DOWN>", 
		"<UP>", 
		"NEWLINE", 
		"ID", 
		"BOOL", 
		"ALIAS", 
		"NOT", 
		"INT", 
		"WS", 
		"'='", 
		"' OR '", 
		"'NOT '", 
		"' AND '", 
		"'('", 
		"')'"
    };

    public const int WS = 10;
    public const int T__16 = 16;
    public const int T__15 = 15;
    public const int NEWLINE = 4;
    public const int T__12 = 12;
    public const int T__11 = 11;
    public const int T__14 = 14;
    public const int T__13 = 13;
    public const int ALIAS = 7;
    public const int BOOL = 6;
    public const int INT = 9;
    public const int NOT = 8;
    public const int ID = 5;
    public const int EOF = -1;

    // delegates
    // delegators



        public ExprParserParser(ITokenStream input)
    		: this(input, new RecognizerSharedState()) {
        }

        public ExprParserParser(ITokenStream input, RecognizerSharedState state)
    		: base(input, state) {
            InitializeCyclicDFAs();

             
        }
        

    override public string[] TokenNames {
		get { return ExprParserParser.tokenNames; }
    }

    override public string GrammarFileName {
		get { return "C:\\Temp\\antlr\\ExprParser.g"; }
    }


    	Hashtable memory = new Hashtable();
    	List<string> _errorMessages = new List<string>();
    	bool _result;



    // $ANTLR start "prog"
    // C:\\Temp\\antlr\\ExprParser.g:25:1: prog : ( stat )+ ;
    public void prog() // throws RecognitionException [1]
    {   
        try 
    	{
            // C:\\Temp\\antlr\\ExprParser.g:25:5: ( ( stat )+ )
            // C:\\Temp\\antlr\\ExprParser.g:25:9: ( stat )+
            {
            	// C:\\Temp\\antlr\\ExprParser.g:25:9: ( stat )+
            	int cnt1 = 0;
            	do 
            	{
            	    int alt1 = 2;
            	    int LA1_0 = input.LA(1);

            	    if ( ((LA1_0 >= NEWLINE && LA1_0 <= ALIAS) || LA1_0 == 13 || LA1_0 == 15) )
            	    {
            	        alt1 = 1;
            	    }


            	    switch (alt1) 
            		{
            			case 1 :
            			    // C:\\Temp\\antlr\\ExprParser.g:25:9: stat
            			    {
            			    	PushFollow(FOLLOW_stat_in_prog55);
            			    	stat();
            			    	state.followingStackPointer--;


            			    }
            			    break;

            			default:
            			    if ( cnt1 >= 1 ) goto loop1;
            		            EarlyExitException eee1 =
            		                new EarlyExitException(1, input);
            		            throw eee1;
            	    }
            	    cnt1++;
            	} while (true);

            	loop1:
            		;	// Stops C# compiler whinging that label 'loop1' has no statements


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "prog"


    // $ANTLR start "stat"
    // C:\\Temp\\antlr\\ExprParser.g:27:1: stat : ( expr NEWLINE | ID '=' expr NEWLINE | NEWLINE );
    public void stat() // throws RecognitionException [1]
    {   
        IToken ID2 = null;
        bool expr1 = default(bool);

        bool expr3 = default(bool);


        try 
    	{
            // C:\\Temp\\antlr\\ExprParser.g:27:5: ( expr NEWLINE | ID '=' expr NEWLINE | NEWLINE )
            int alt2 = 3;
            switch ( input.LA(1) ) 
            {
            case BOOL:
            case ALIAS:
            case 13:
            case 15:
            	{
                alt2 = 1;
                }
                break;
            case ID:
            	{
                int LA2_2 = input.LA(2);

                if ( (LA2_2 == 11) )
                {
                    alt2 = 2;
                }
                else if ( (LA2_2 == NEWLINE || LA2_2 == 12 || LA2_2 == 14) )
                {
                    alt2 = 1;
                }
                else 
                {
                    NoViableAltException nvae_d2s2 =
                        new NoViableAltException("", 2, 2, input);

                    throw nvae_d2s2;
                }
                }
                break;
            case NEWLINE:
            	{
                alt2 = 3;
                }
                break;
            	default:
            	    NoViableAltException nvae_d2s0 =
            	        new NoViableAltException("", 2, 0, input);

            	    throw nvae_d2s0;
            }

            switch (alt2) 
            {
                case 1 :
                    // C:\\Temp\\antlr\\ExprParser.g:28:5: expr NEWLINE
                    {
                    	PushFollow(FOLLOW_expr_in_stat87);
                    	expr1 = expr();
                    	state.followingStackPointer--;

                    	Match(input,NEWLINE,FOLLOW_NEWLINE_in_stat89); 

                    	    		_result = expr1;
                    		

                    }
                    break;
                case 2 :
                    // C:\\Temp\\antlr\\ExprParser.g:31:9: ID '=' expr NEWLINE
                    {
                    	ID2=(IToken)Match(input,ID,FOLLOW_ID_in_stat101); 
                    	Match(input,11,FOLLOW_11_in_stat103); 
                    	PushFollow(FOLLOW_expr_in_stat105);
                    	expr3 = expr();
                    	state.followingStackPointer--;

                    	Match(input,NEWLINE,FOLLOW_NEWLINE_in_stat107); 

                    	        	memory.Add(((ID2 != null) ? ID2.Text : null), bool.Parse(expr3.ToString()));
                    	       	

                    }
                    break;
                case 3 :
                    // C:\\Temp\\antlr\\ExprParser.g:35:9: NEWLINE
                    {
                    	Match(input,NEWLINE,FOLLOW_NEWLINE_in_stat127); 

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "stat"


    // $ANTLR start "expr"
    // C:\\Temp\\antlr\\ExprParser.g:38:1: expr returns [bool value] : (e= multExpr ( ' OR ' e= multExpr )* | 'NOT ' e= expr );
    public bool expr() // throws RecognitionException [1]
    {   
        bool value = default(bool);

        bool e = default(bool);


        try 
    	{
            // C:\\Temp\\antlr\\ExprParser.g:39:5: (e= multExpr ( ' OR ' e= multExpr )* | 'NOT ' e= expr )
            int alt4 = 2;
            int LA4_0 = input.LA(1);

            if ( ((LA4_0 >= ID && LA4_0 <= ALIAS) || LA4_0 == 15) )
            {
                alt4 = 1;
            }
            else if ( (LA4_0 == 13) )
            {
                alt4 = 2;
            }
            else 
            {
                NoViableAltException nvae_d4s0 =
                    new NoViableAltException("", 4, 0, input);

                throw nvae_d4s0;
            }
            switch (alt4) 
            {
                case 1 :
                    // C:\\Temp\\antlr\\ExprParser.g:39:9: e= multExpr ( ' OR ' e= multExpr )*
                    {
                    	PushFollow(FOLLOW_multExpr_in_expr152);
                    	e = multExpr();
                    	state.followingStackPointer--;


                    	    		value =  e;
                    	    	
                    	// C:\\Temp\\antlr\\ExprParser.g:43:9: ( ' OR ' e= multExpr )*
                    	do 
                    	{
                    	    int alt3 = 2;
                    	    int LA3_0 = input.LA(1);

                    	    if ( (LA3_0 == 12) )
                    	    {
                    	        alt3 = 1;
                    	    }


                    	    switch (alt3) 
                    		{
                    			case 1 :
                    			    // C:\\Temp\\antlr\\ExprParser.g:43:13: ' OR ' e= multExpr
                    			    {
                    			    	Match(input,12,FOLLOW_12_in_expr174); 
                    			    	PushFollow(FOLLOW_multExpr_in_expr178);
                    			    	e = multExpr();
                    			    	state.followingStackPointer--;

                    			    	value =  value || e;

                    			    }
                    			    break;

                    			default:
                    			    goto loop3;
                    	    }
                    	} while (true);

                    	loop3:
                    		;	// Stops C# compiler whining that label 'loop3' has no statements


                    }
                    break;
                case 2 :
                    // C:\\Temp\\antlr\\ExprParser.g:45:7: 'NOT ' e= expr
                    {
                    	Match(input,13,FOLLOW_13_in_expr207); 
                    	PushFollow(FOLLOW_expr_in_expr211);
                    	e = expr();
                    	state.followingStackPointer--;


                    	    	value =  ! e;
                    	    

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return value;
    }
    // $ANTLR end "expr"


    // $ANTLR start "multExpr"
    // C:\\Temp\\antlr\\ExprParser.g:51:1: multExpr returns [bool value] : e= atom ( ' AND ' e= atom )* ;
    public bool multExpr() // throws RecognitionException [1]
    {   
        bool value = default(bool);

        bool e = default(bool);


        try 
    	{
            // C:\\Temp\\antlr\\ExprParser.g:52:5: (e= atom ( ' AND ' e= atom )* )
            // C:\\Temp\\antlr\\ExprParser.g:52:9: e= atom ( ' AND ' e= atom )*
            {
            	PushFollow(FOLLOW_atom_in_multExpr242);
            	e = atom();
            	state.followingStackPointer--;

            	value =  e;
            	// C:\\Temp\\antlr\\ExprParser.g:52:37: ( ' AND ' e= atom )*
            	do 
            	{
            	    int alt5 = 2;
            	    int LA5_0 = input.LA(1);

            	    if ( (LA5_0 == 14) )
            	    {
            	        alt5 = 1;
            	    }


            	    switch (alt5) 
            		{
            			case 1 :
            			    // C:\\Temp\\antlr\\ExprParser.g:52:38: ' AND ' e= atom
            			    {
            			    	Match(input,14,FOLLOW_14_in_multExpr247); 
            			    	PushFollow(FOLLOW_atom_in_multExpr251);
            			    	e = atom();
            			    	state.followingStackPointer--;

            			    	value =  value && e;

            			    }
            			    break;

            			default:
            			    goto loop5;
            	    }
            	} while (true);

            	loop5:
            		;	// Stops C# compiler whining that label 'loop5' has no statements


            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return value;
    }
    // $ANTLR end "multExpr"


    // $ANTLR start "atom"
    // C:\\Temp\\antlr\\ExprParser.g:55:1: atom returns [bool value] : ( BOOL | ALIAS | ID | '(' expr ')' );
    public bool atom() // throws RecognitionException [1]
    {   
        bool value = default(bool);

        IToken BOOL4 = null;
        IToken ALIAS5 = null;
        IToken ID6 = null;
        bool expr7 = default(bool);


        try 
    	{
            // C:\\Temp\\antlr\\ExprParser.g:56:5: ( BOOL | ALIAS | ID | '(' expr ')' )
            int alt6 = 4;
            switch ( input.LA(1) ) 
            {
            case BOOL:
            	{
                alt6 = 1;
                }
                break;
            case ALIAS:
            	{
                alt6 = 2;
                }
                break;
            case ID:
            	{
                alt6 = 3;
                }
                break;
            case 15:
            	{
                alt6 = 4;
                }
                break;
            	default:
            	    NoViableAltException nvae_d6s0 =
            	        new NoViableAltException("", 6, 0, input);

            	    throw nvae_d6s0;
            }

            switch (alt6) 
            {
                case 1 :
                    // C:\\Temp\\antlr\\ExprParser.g:56:9: BOOL
                    {
                    	BOOL4=(IToken)Match(input,BOOL,FOLLOW_BOOL_in_atom279); 
                    	value =  bool.Parse(((BOOL4 != null) ? BOOL4.Text : null));

                    }
                    break;
                case 2 :
                    // C:\\Temp\\antlr\\ExprParser.g:57:7: ALIAS
                    {
                    	ALIAS5=(IToken)Match(input,ALIAS,FOLLOW_ALIAS_in_atom289); 
                    	 
                    	    		value =  EvaluteByAlias(((ALIAS5 != null) ? ALIAS5.Text : null));     		
                    	    		

                    }
                    break;
                case 3 :
                    // C:\\Temp\\antlr\\ExprParser.g:60:9: ID
                    {
                    	ID6=(IToken)Match(input,ID,FOLLOW_ID_in_atom301); 

                    	        object tmp = memory[((ID6 != null) ? ID6.Text : null)];
                    	        if ( tmp!=null ) value =  (bool)tmp;
                    	        else _errorMessages.Add("undefined variable "+((ID6 != null) ? ID6.Text : null));
                    	        

                    }
                    break;
                case 4 :
                    // C:\\Temp\\antlr\\ExprParser.g:66:9: '(' expr ')'
                    {
                    	Match(input,15,FOLLOW_15_in_atom321); 
                    	PushFollow(FOLLOW_expr_in_atom323);
                    	expr7 = expr();
                    	state.followingStackPointer--;

                    	Match(input,16,FOLLOW_16_in_atom325); 
                    	value =  expr7;

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return value;
    }
    // $ANTLR end "atom"

    // Delegated rules


	private void InitializeCyclicDFAs()
	{
	}

 

    public static readonly BitSet FOLLOW_stat_in_prog55 = new BitSet(new ulong[]{0x000000000000A0F2UL});
    public static readonly BitSet FOLLOW_expr_in_stat87 = new BitSet(new ulong[]{0x0000000000000010UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_stat89 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ID_in_stat101 = new BitSet(new ulong[]{0x0000000000000800UL});
    public static readonly BitSet FOLLOW_11_in_stat103 = new BitSet(new ulong[]{0x000000000000A0E0UL});
    public static readonly BitSet FOLLOW_expr_in_stat105 = new BitSet(new ulong[]{0x0000000000000010UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_stat107 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_stat127 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_multExpr_in_expr152 = new BitSet(new ulong[]{0x0000000000001002UL});
    public static readonly BitSet FOLLOW_12_in_expr174 = new BitSet(new ulong[]{0x00000000000080E0UL});
    public static readonly BitSet FOLLOW_multExpr_in_expr178 = new BitSet(new ulong[]{0x0000000000001002UL});
    public static readonly BitSet FOLLOW_13_in_expr207 = new BitSet(new ulong[]{0x000000000000A0E0UL});
    public static readonly BitSet FOLLOW_expr_in_expr211 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_atom_in_multExpr242 = new BitSet(new ulong[]{0x0000000000004002UL});
    public static readonly BitSet FOLLOW_14_in_multExpr247 = new BitSet(new ulong[]{0x00000000000080E0UL});
    public static readonly BitSet FOLLOW_atom_in_multExpr251 = new BitSet(new ulong[]{0x0000000000004002UL});
    public static readonly BitSet FOLLOW_BOOL_in_atom279 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ALIAS_in_atom289 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ID_in_atom301 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_15_in_atom321 = new BitSet(new ulong[]{0x000000000000A0E0UL});
    public static readonly BitSet FOLLOW_expr_in_atom323 = new BitSet(new ulong[]{0x0000000000010000UL});
    public static readonly BitSet FOLLOW_16_in_atom325 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}