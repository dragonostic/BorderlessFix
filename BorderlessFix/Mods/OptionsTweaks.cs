using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace BorderlessFix.Mods;

public class OptionsTweaks {
    public class OptionsMenuModifier() : IScriptMod {
        public bool ShouldRun(string path) => path == "res://Scenes/Singletons/OptionsMenu/options_menu.gdc";

        public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens) {
            MultiTokenWaiter setAlwaysOnTop = new([
                t => t is IdentifierToken { Name: "set_window_always_on_top"},
                t => t.Type is TokenType.ParenthesisOpen,
                t => t is IdentifierToken { Name: "PlayerData"},
                t => t.Type is TokenType.Period,
                t => t is IdentifierToken { Name: "player_options"},
                t => t.Type is TokenType.Period,
                t => t is IdentifierToken { Name: "fullscreen"}
            ]);

            MultiTokenWaiter windowedCheck = new([
                t => t is ConstantToken {Value: IntVariant {Value: 1}},
                t => t.Type is TokenType.Colon,
                t => t.Type is TokenType.Newline,
                t => t is IdentifierToken { Name: "OS"},
                t => t.Type is TokenType.Period,
                t => t is IdentifierToken { Name: "window_size"},
                t => t.Type is TokenType.OpAssign
            ]);

            TokenWaiter maximizeWindow = new(t => t is IdentifierToken { Name: "window_maximized" });

            MultiTokenWaiter centerWindow = new([
                t => t is IdentifierToken { Name: "fullscreen"},
                t => t.Type is TokenType.OpEqual,
                t => t is ConstantToken {Value: IntVariant {Value: 0}},
                t => t.Type is TokenType.Colon,
            ]);

            TokenConsumer newlineConsumer = new(t => t.Type is TokenType.Newline);

            foreach (var token in tokens) {
                if (newlineConsumer.Check(token)) {
                    continue;
                } else if (newlineConsumer.Ready) {
                    yield return token;
                    newlineConsumer.Reset();
                }

                if (setAlwaysOnTop.Check(token)) {
                    yield return token;

                    yield return new Token(TokenType.OpEqual);
                    yield return new ConstantToken(new IntVariant(2));
                    yield return new Token(TokenType.ParenthesisClose);

                    newlineConsumer.SetReady();
                } else if (maximizeWindow.Check(token)) {
                    yield return token;

                    yield return new Token(TokenType.OpAssign);
                    yield return new ConstantToken(new BoolVariant(false));

                    newlineConsumer.SetReady();
                } else if (windowedCheck.Check(token)) {
                    yield return token;

                    yield return new IdentifierToken("OS");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("get_screen_size");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.OpAdd);
                    yield return new Token(TokenType.BuiltInType, (uint?) VariantType.Vector2);
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new ConstantToken(new IntVariant(0));
                    yield return new Token(TokenType.Comma);
                    yield return new ConstantToken(new IntVariant(1));
                    yield return new Token(TokenType.ParenthesisClose);

                    newlineConsumer.SetReady();
                } else if (centerWindow.Check(token)) {
                    yield return token;

                    yield return new Token(TokenType.Newline, 2);
                    yield return new IdentifierToken("OS");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("center_window");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new Token(TokenType.ParenthesisClose);
                } else {
                    yield return token;
                }
            }
        }
    }
}