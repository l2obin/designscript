using AssociativeAST = ProtoCore.AST.AssociativeAST;
using ImperativeAST = ProtoCore.AST.ImperativeAST;
using ProtoCore.AST;
using ProtoCore.DSASM.Mirror;
using ProtoCore.Lang;
using ProtoTestFx.TD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscoveryChannel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TestFrameWork thisTest;

        public MainWindow()
        {
            InitializeComponent();
            thisTest = new TestFrameWork();
        }

        private void RunCode(object sender, RoutedEventArgs e)
        {
            AssociativeAST.CodeBlockNode commentNode = new AssociativeAST.CodeBlockNode();
            string code = @txtInput.Text;
            ProtoCore.AST.Node codeBlockNode = GraphToDSCompiler.GraphUtilities.Parse(code, out commentNode);
            List<ProtoCore.AST.Node> nodes = ProtoCore.Utils.ParserUtils.GetAstNodes(codeBlockNode);

            // Serialize the AST tree and print it out
            /*
            ASTNodes ast = new ASTNodes(nodes);
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ASTNodes));
            var stringwriter = new System.IO.StringWriter();
            x.Serialize(stringwriter, ast);
            txtOutput.Text = stringwriter.ToString();
            */
        }

        public class ASTNodes
        {
            public List<Node> Nodes;

            public ASTNodes()
            {
            }

            public ASTNodes(List<Node> nodes)
            {
                this.Nodes = nodes;
            }
        }

        private void RunAST(object sender, RoutedEventArgs e)
        {
            List<AssociativeAST.AssociativeNode> astList = new List<AssociativeAST.AssociativeNode>();
            /*
            // a = { { 0, 1 }, { 3, 4, 5 } };
            int[] input1 = { 0, 1 };
            int[] input2 = { 2, 3, 4 };
            List<ProtoCore.AST.AssociativeAST.AssociativeNode> arrayList = new List<ProtoCore.AST.AssociativeAST.AssociativeNode>();
            arrayList.Add(CreateExprListNodeFromArray(input1));
            arrayList.Add(CreateExprListNodeFromArray(input2));

            ProtoCore.AST.AssociativeAST.ExprListNode arrayExpr = new ProtoCore.AST.AssociativeAST.ExprListNode { list = arrayList };

            ProtoCore.AST.AssociativeAST.BinaryExpressionNode declareNodeA = new ProtoCore.AST.AssociativeAST.BinaryExpressionNode(
                new ProtoCore.AST.AssociativeAST.IdentifierNode("a"),
                arrayExpr,
                ProtoCore.DSASM.Operator.assign);

            astList.Add(declareNodeA);

            // b = 2;
            ProtoCore.AST.AssociativeAST.BinaryExpressionNode declareNodeB = new ProtoCore.AST.AssociativeAST.BinaryExpressionNode(
                new ProtoCore.AST.AssociativeAST.IdentifierNode("b"),
                new ProtoCore.AST.AssociativeAST.IntNode("2"),
                ProtoCore.DSASM.Operator.assign);
            astList.Add(declareNodeB);

            // c = a[1][b];
            ProtoCore.AST.AssociativeAST.IdentifierNode nodeARHS = new ProtoCore.AST.AssociativeAST.IdentifierNode("a");
            nodeARHS.ArrayDimensions = new ProtoCore.AST.AssociativeAST.ArrayNode
            {
                Expr = new ProtoCore.AST.AssociativeAST.IntNode("1"),
                Type = new ProtoCore.AST.AssociativeAST.ArrayNode {
                    Expr = new ProtoCore.AST.AssociativeAST.IdentifierNode("b")
                }
            };

            ProtoCore.AST.AssociativeAST.BinaryExpressionNode nodeARHSAssignment = new ProtoCore.AST.AssociativeAST.BinaryExpressionNode(
                new ProtoCore.AST.AssociativeAST.IdentifierNode("c"),
                nodeARHS,
                ProtoCore.DSASM.Operator.assign);

            astList.Add(nodeARHSAssignment);
            */

            // a = [Imperative] {
            //    b = -7;
            //    c = b + 12;
            // }

            // a = [Imperative] { }
            List<ImperativeAST.ImperativeNode> imperativeASTList = new List<ImperativeAST.ImperativeNode>();

            AssociativeAST.BinaryExpressionNode declareNodeA = new AssociativeAST.BinaryExpressionNode(
                new ProtoCore.AST.AssociativeAST.IdentifierNode("a"),
                new ProtoCore.AST.AssociativeAST.LanguageBlockNode
                {
                    CodeBlockNode = new ProtoCore.AST.ImperativeAST.CodeBlockNode { Body = imperativeASTList }
                },
                ProtoCore.DSASM.Operator.assign);
            astList.Add(declareNodeA);

            // b = -7;
            ProtoCore.AST.ImperativeAST.BinaryExpressionNode assign1 = new ProtoCore.AST.ImperativeAST.BinaryExpressionNode{
                LeftNode = new ProtoCore.AST.ImperativeAST.IdentifierNode{ Name = "b" },
                RightNode = new ProtoCore.AST.ImperativeAST.IntNode{ value = "-7" },
                Optr = ProtoCore.DSASM.Operator.assign
            };
            imperativeASTList.Add(assign1);

            // c = b + 12;
            ProtoCore.AST.ImperativeAST.BinaryExpressionNode assign2 = new ProtoCore.AST.ImperativeAST.BinaryExpressionNode
            {
                LeftNode = new ProtoCore.AST.ImperativeAST.IdentifierNode { Name = "c" },
                RightNode = new ImperativeAST.BinaryExpressionNode
                {
                    LeftNode = new ProtoCore.AST.ImperativeAST.IdentifierNode { Name = "b" },
                    RightNode = new ProtoCore.AST.ImperativeAST.IntNode { value = "12" },
                    Optr = ProtoCore.DSASM.Operator.add
                },
                Optr = ProtoCore.DSASM.Operator.assign
            };
            imperativeASTList.Add(assign2);

            // Verify the results
            ExecutionMirror mirror = thisTest.RunASTSource(astList);
            Obj o = mirror.GetValue("a");
            Console.WriteLine(o.Payload);
        }

        private AssociativeAST.BinaryExpressionNode CreateDeclareArrayNode(string name, int[] intList)
        {
            AssociativeAST.ExprListNode expr = CreateExprListNodeFromArray(intList);

            AssociativeAST.BinaryExpressionNode declareArrayNode = new AssociativeAST.BinaryExpressionNode(
                new AssociativeAST.IdentifierNode(name),
                expr,
                ProtoCore.DSASM.Operator.assign);

            return declareArrayNode;
        }

        private AssociativeAST.ExprListNode CreateExprListNodeFromArray(int[] intList)
        {
            List<AssociativeAST.AssociativeNode> listIntNode = new List<AssociativeAST.AssociativeNode>();
            for (int i = 0; i < intList.Length; i++)
                listIntNode.Add(new AssociativeAST.IntNode(intList[i].ToString()));

            AssociativeAST.ExprListNode expr = new AssociativeAST.ExprListNode { list = listIntNode };

            return expr;
        }

        //private ProtoCore.AST.AssociativeAST.BinaryExpressionNode
    }
}
