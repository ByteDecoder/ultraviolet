﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TwistedLogik.Ultraviolet.UI.Presentation.Uvss.Syntax;
using TwistedLogik.Ultraviolet.UI.Presentation.Uvss.Testing;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Uvss.Tests
{
    [TestClass]
    public class UvssParserTests : UvssTestFramework
    {
        [TestMethod]
        public void UvssParser_CorrectlyCalculatesNodeSpan()
        {
            var document = UvssParser.Parse(
                "\r\n" +
                "#foo {}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var selector = ruleSet.Selectors[0];
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveSpan(0, 7); // "\r\n#foo "

            var selectorPart = selector.Components[0] as UvssSelectorPartSyntax;
            TheResultingNode(selectorPart)
                .ShouldBePresent();

            var selectorSubPart = selectorPart.SubParts[0];
            TheResultingNode(selectorSubPart)
                .ShouldBePresent();

            var selectorHashToken = selectorSubPart.LeadingQualifierToken;
            TheResultingNode(selectorHashToken)
                .ShouldBePresent()
                .ShouldHaveSpan(0, 3); // "\r\n#"

            var selectorIdentifier = selectorSubPart.SubPartIdentifier;
            TheResultingNode(selectorIdentifier)
                .ShouldBePresent()
                .ShouldHaveSpan(3, 4); // "foo ";
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesCompleteGarbage()
        {
            var document = UvssParser.Parse(
                "${}$% 4u95 47698406t26% ^UY(%U^ %(^257 629576%${%$[5p45 }${% 1%($%)!$(%90");

            TheResultingNode(document)
                .ShouldHaveFullString("${}$% 4u95 47698406t26% ^UY(%U^ %(^257 629576%${%$[5p45 }${% 1%($%)!$(%90", includeTrivia: true);
        }

        [TestMethod]
        public void UvssParser_CreatesSkippedTokensTrivia_WhenSymbolCannotBeParsed()
        {
            var document = UvssParser.Parse(
                "#foo { &&& }");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var body = ruleSet.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);

            var openCurlyBrace = body.OpenCurlyBraceToken;
            TheResultingNode(openCurlyBrace)
                .ShouldBePresent();

            var openCurlyBraceTrivia = openCurlyBrace.GetTrailingTrivia();
            TheResultingNode(openCurlyBraceTrivia)
                .ShouldSatisfyTheCondition(x => x.IsList)
                .ShouldSatisfyTheCondition(x => x.SlotCount == 5)
                .ShouldHaveFullString(" &&& ");

            var closeCurlyBrace = body.CloseCurlyBraceToken;
            TheResultingNode(closeCurlyBrace)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CreatesSkippedTokensTrivia_WhenSymbolIsUnexpected()
        {
            var document = UvssParser.Parse(
                "} #foo {}");

            var emptyStatement = document.Content[0] as UvssEmptyStatementSyntax;

            var trivia = emptyStatement.GetLeadingTrivia();

            TheResultingNode(emptyStatement)
                .ShouldBePresent()
                .ShouldHaveFullString("} ", includeTrivia: true);

            var ruleSet = document.Content[1] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo {}");
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesEmptyDocument()
        {
            var document = UvssParser.Parse(
                String.Empty);

            TheResultingObject(document).ShouldSatisfyTheCondition(x =>
                x != null &&
                x.Position == 0 &&
                x.FullWidth == 0 &&
                x.Content.Count == 0 &&
                x.EndOfFileToken != null);
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesIncompleteRuleSet()
        {
            var document = UvssParser.Parse(
                "#foo");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var selector = ruleSet.Selectors[0] as UvssSelectorBaseSyntax;
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo");

            var body = ruleSet.Body as UvssBlockSyntax;
            TheResultingNode(body)
                .ShouldBeMissing();

            var openCurlyBrace = body.OpenCurlyBraceToken;
            TheResultingNode(openCurlyBrace)
                .ShouldBeMissing();

            var closeCurlyBrace = body.CloseCurlyBraceToken;
            TheResultingNode(closeCurlyBrace)
                .ShouldBeMissing();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesIncompleteRuleSet_WithOpenCurlyBrace()
        {
            var document = UvssParser.Parse(
                "#foo {");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var selector = ruleSet.Selectors[0] as UvssSelectorBaseSyntax;
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo");

            var body = ruleSet.Body as UvssBlockSyntax;
            TheResultingNode(body)
                .ShouldBePresent();

            var openCurlyBrace = body.OpenCurlyBraceToken;
            TheResultingNode(openCurlyBrace)
                .ShouldBePresent();

            var closeCurlyBrace = body.CloseCurlyBraceToken;
            TheResultingNode(closeCurlyBrace)
                .ShouldBeMissing();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesIncompleteRuleSet_WithCloseCurlyBrace()
        {
            var document = UvssParser.Parse(
                "#foo }");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var selector = ruleSet.Selectors[0] as UvssSelectorBaseSyntax;
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo");

            var body = ruleSet.Body as UvssBlockSyntax;
            TheResultingNode(body)
                .ShouldBePresent();

            var openCurlyBrace = body.OpenCurlyBraceToken;
            TheResultingNode(openCurlyBrace)
                .ShouldBeMissing();

            var closeCurlyBrace = body.CloseCurlyBraceToken;
            TheResultingNode(closeCurlyBrace)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesIncompleteRuleSet_FollowedByCompleteRuleSet()
        {
            var document = UvssParser.Parse(
                "#foo { #bar {}");

            var ruleSet0 = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet0)
                .ShouldBePresent();

            var ruleSet0Selector = ruleSet0.Selectors[0] as UvssSelectorBaseSyntax;
            TheResultingNode(ruleSet0Selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo");

            var ruleSet0CloseCurlyBrace = ruleSet0.Body.CloseCurlyBraceToken;
            TheResultingNode(ruleSet0CloseCurlyBrace)
                .ShouldBeMissing();

            var ruleSet1 = document.Content[1] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet1)
                .ShouldBePresent();

            var ruleSet1Selector = ruleSet1.Selectors[0] as UvssSelectorBaseSyntax;
            TheResultingNode(ruleSet1Selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#bar");

            var ruleSet1CloseCurlyBrace = ruleSet1.Body.CloseCurlyBraceToken;
            TheResultingNode(ruleSet1CloseCurlyBrace)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesRuleSet_WhenEmpty_WithOneSimpleSelector()
        {
            var document = UvssParser.Parse(
                "#foo {}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var selector = ruleSet.Selectors[0];
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo");

            var navigationExpression = selector.NavigationExpression;
            TheResultingNode(navigationExpression)
                .ShouldBeNull();
                        
            var body = ruleSet.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);
        }
        
        [TestMethod]
        public void UvssParser_CorrectlyParsesRuleSet_WhenEmpty_WithOneSimpleSelector_WithNavigationExpression()
        {
            var document = UvssParser.Parse(
                "#foo | bar as Baz {}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var selector = ruleSet.Selectors[0];
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo | bar as Baz");

            var navigationExpression = selector.NavigationExpression;
            TheResultingNode(navigationExpression)
                .ShouldBePresent()
                .ShouldHaveFullString("| bar as Baz");

            var body = ruleSet.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesRuleSet_WhenEmpty_WithOneComplexSelector()
        {
            var document = UvssParser.Parse(
                "#foo.bar > baz!:pseudo >> * {}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var selector = ruleSet.Selectors[0];
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo.bar > baz!:pseudo >> *");

            var selectorComponent0 = selector.Components[0] as UvssSelectorPartSyntax;
            TheResultingNode(selectorComponent0)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo.bar");

            var selectorComponent1 = selector.Components[1] as SyntaxToken;
            TheResultingNode(selectorComponent1)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.GreaterThanToken);

            var selectorComponent2 = selector.Components[2] as UvssSelectorPartSyntax;
            TheResultingNode(selectorComponent2)
                .ShouldBePresent()
                .ShouldHaveFullString("baz!:pseudo");

            var selectorComponent3 = selector.Components[3] as SyntaxToken;
            TheResultingNode(selectorComponent3)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.GreaterThanGreaterThanToken);

            var selectorComponent4 = selector.Components[4] as UvssUniversalSelectorPartSyntax;
            TheResultingNode(selectorComponent4)
                .ShouldBePresent()
                .ShouldHaveFullString("*");
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesRuleSet_WhenEmpty_WithMultipleSimpleSelectors()
        {
            var document = UvssParser.Parse(
                "#foo, #bar, #baz {}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();
            
            var selector0 = ruleSet.Selectors[0];
            TheResultingNode(selector0)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo");

            var selector1 = ruleSet.Selectors[1];
            TheResultingNode(selector1)
                .ShouldBePresent()
                .ShouldHaveFullString("#bar");

            var selector2 = ruleSet.Selectors[2];
            TheResultingNode(selector2)
                .ShouldBePresent()
                .ShouldHaveFullString("#baz");
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesRuleSet_WhenEmpty_WithOneComplexSelectors()
        {
            var document = UvssParser.Parse(
                "#foo.bar > baz!:pseudo >> *, .foo.bar .baz .qux {}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var selector0 = ruleSet.Selectors[0];
            TheResultingNode(selector0)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo.bar > baz!:pseudo >> *");

            var selector0Component0 = selector0.Components[0] as UvssSelectorPartSyntax;
            TheResultingNode(selector0Component0)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo.bar");

            var selector0Component1 = selector0.Components[1] as SyntaxToken;
            TheResultingNode(selector0Component1)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.GreaterThanToken);

            var selector0Component2 = selector0.Components[2] as UvssSelectorPartSyntax;
            TheResultingNode(selector0Component2)
                .ShouldBePresent()
                .ShouldHaveFullString("baz!:pseudo");

            var selector0Component3 = selector0.Components[3] as SyntaxToken;
            TheResultingNode(selector0Component3)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.GreaterThanGreaterThanToken);
            
            var selector0Component4 = selector0.Components[4] as UvssUniversalSelectorPartSyntax;
            TheResultingNode(selector0Component4)
                .ShouldBePresent()
                .ShouldHaveFullString("*");

            var selector1 = ruleSet.Selectors[1];
            TheResultingNode(selector1)
                .ShouldBePresent()
                .ShouldHaveFullString(".foo.bar .baz .qux");

            var selector1Component0 = selector1.Components[0] as UvssSelectorPartSyntax;
            TheResultingNode(selector1Component0)
                .ShouldBePresent()
                .ShouldHaveFullString(".foo.bar");

            var selector1Component1 = selector1.Components[1] as SyntaxToken;
            TheResultingNode(selector1Component1)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.SpaceToken);

            var selector1Component2 = selector1.Components[2] as UvssSelectorPartSyntax;
            TheResultingNode(selector1Component2)
                .ShouldBePresent()
                .ShouldHaveFullString(".baz");

            var selector1Component3 = selector1.Components[3] as SyntaxToken;
            TheResultingNode(selector1Component3)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.SpaceToken);

            var selector1Component4 = selector1.Components[4] as UvssSelectorPartSyntax;
            TheResultingNode(selector1Component4)
                .ShouldBePresent()
                .ShouldHaveFullString(".qux");
        }
        
        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleStylingRule()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\tfoo: bar baz;\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var rule = ruleSet.Body.Content[0] as UvssRuleSyntax;
            TheResultingNode(rule)
                .ShouldBePresent();

            var propertyName = rule.PropertyName;
            TheResultingNode(propertyName)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var colonToken = rule.ColonToken;
            TheResultingNode(colonToken)
                .ShouldBePresent();

            var value = rule.Value;
            TheResultingNode(value)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.PropertyValue)
                .ShouldHaveFullString("bar baz");

            var qualifierToken = rule.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBeNull();

            var semiColonToken = rule.SemiColonToken;
            TheResultingNode(semiColonToken)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleStylingRule_WhenImportant()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\tfoo: bar baz !important;\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var rule = ruleSet.Body.Content[0] as UvssRuleSyntax;
            TheResultingNode(rule)
                .ShouldBePresent();

            var propertyName = rule.PropertyName;
            TheResultingNode(propertyName)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var colonToken = rule.ColonToken;
            TheResultingNode(colonToken)
                .ShouldBePresent();

            var value = rule.Value;
            TheResultingNode(value)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.PropertyValue)
                .ShouldHaveFullString("bar baz");

            var qualifierToken = rule.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.ImportantKeyword);

            var semiColonToken = rule.SemiColonToken;
            TheResultingNode(semiColonToken)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesMultipleStylingRules()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\tfoo: bar baz;\r\n" +
                    "\tabc: 123 456;\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var rule0 = ruleSet.Body.Content[0] as UvssRuleSyntax;
            TheResultingNode(rule0)
                .ShouldBePresent()
                .ShouldHaveFullString("foo: bar baz;");

            var rule1 = ruleSet.Body.Content[1] as UvssRuleSyntax;
            TheResultingNode(rule1)
                .ShouldBePresent()
                .ShouldHaveFullString("abc: 123 456;");
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesMultipleStylingRules_WhenImportant()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\tfoo: bar baz !important;\r\n" +
                    "\tabc: 123 456 !important;\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var rule0 = ruleSet.Body.Content[0] as UvssRuleSyntax;
            TheResultingNode(rule0)
                .ShouldBePresent()
                .ShouldHaveFullString("foo: bar baz !important;");

            var rule1 = ruleSet.Body.Content[1] as UvssRuleSyntax;
            TheResultingNode(rule1)
                .ShouldBePresent()
                .ShouldHaveFullString("abc: 123 456 !important;");
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleVisualTransition()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttransition (common, pressed, normal): foo bar baz;\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var transition = ruleSet.Body.Content[0] as UvssTransitionSyntax;
            TheResultingNode(transition)
                .ShouldBePresent();

            var transitionKeyword = transition.TransitionKeyword;
            TheResultingNode(transitionKeyword)
                .ShouldBePresent();

            var transitionArgList = transition.ArgumentList;
            TheResultingNode(transitionArgList)
                .ShouldBePresent();

            var arg0 = transitionArgList.Arguments[0] as UvssIdentifierBaseSyntax;
            TheResultingNode(arg0)
                .ShouldBePresent()
                .ShouldHaveFullString("common");

            var arg1 = transitionArgList.Arguments[1] as UvssIdentifierBaseSyntax;
            TheResultingNode(arg1)
                .ShouldBePresent()
                .ShouldHaveFullString("pressed");

            var arg2 = transitionArgList.Arguments[2] as UvssIdentifierBaseSyntax;
            TheResultingNode(arg2)
                .ShouldBePresent()
                .ShouldHaveFullString("normal");

            var colonToken = transition.ColonToken;
            TheResultingNode(colonToken)
                .ShouldBePresent();

            var value = transition.Value;
            TheResultingNode(value)
                .ShouldBePresent()
                .ShouldHaveFullString("foo bar baz");
            
            var qualifierToken = transition.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBeNull();

            var semiColonToken = transition.SemiColonToken;
            TheResultingNode(semiColonToken)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleVisualTransition_WhenImportant()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttransition (common, pressed, normal): foo bar baz !important;\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var transition = ruleSet.Body.Content[0] as UvssTransitionSyntax;
            TheResultingNode(transition)
                .ShouldBePresent();

            var transitionKeyword = transition.TransitionKeyword;
            TheResultingNode(transitionKeyword)
                .ShouldBePresent();

            var transitionArgList = transition.ArgumentList;
            TheResultingNode(transitionArgList)
                .ShouldBePresent();

            var arg0 = transitionArgList.Arguments[0] as UvssIdentifierBaseSyntax;
            TheResultingNode(arg0)
                .ShouldBePresent()
                .ShouldHaveFullString("common");

            var arg1 = transitionArgList.Arguments[1] as UvssIdentifierBaseSyntax;
            TheResultingNode(arg1)
                .ShouldBePresent()
                .ShouldHaveFullString("pressed");

            var arg2 = transitionArgList.Arguments[2] as UvssIdentifierBaseSyntax;
            TheResultingNode(arg2)
                .ShouldBePresent()
                .ShouldHaveFullString("normal");

            var colonToken = transition.ColonToken;
            TheResultingNode(colonToken)
                .ShouldBePresent();

            var value = transition.Value;
            TheResultingNode(value)
                .ShouldBePresent()
                .ShouldHaveFullString("foo bar baz");

            var qualifierToken = transition.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.ImportantKeyword);

            var semiColonToken = transition.SemiColonToken;
            TheResultingNode(semiColonToken)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSinglePropertyTrigger()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttrigger property foo = { bar baz }\r\n" +
                    "\t{ }\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var propertyTrigger = ruleSet.Body.Content[0] as UvssPropertyTriggerSyntax;
            TheResultingNode(propertyTrigger)
                .ShouldBePresent();

            var triggerKeyword = propertyTrigger.TriggerKeyword;
            TheResultingNode(triggerKeyword)
                .ShouldBePresent();

            var propertyKeyword = propertyTrigger.PropertyKeyword;
            TheResultingNode(propertyKeyword)
                .ShouldBePresent();

            var condition = propertyTrigger.Conditions[0];
            TheResultingNode(condition)
                .ShouldBePresent();

            var conditionPropertyName = condition.PropertyName;
            TheResultingNode(conditionPropertyName)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var conditionComparisonOperatorToken = condition.ComparisonOperatorToken;
            TheResultingNode(conditionComparisonOperatorToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.EqualsToken);

            var conditionValue = condition.PropertyValue;
            TheResultingNode(conditionValue)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Value == "bar baz");

            var qualifierToken = propertyTrigger.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBeNull();

            var body = propertyTrigger.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSinglePropertyTrigger_WhenImportant()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttrigger property foo = { bar baz } !important\r\n" +
                    "\t{ }\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var propertyTrigger = ruleSet.Body.Content[0] as UvssPropertyTriggerSyntax;
            TheResultingNode(propertyTrigger)
                .ShouldBePresent();

            var triggerKeyword = propertyTrigger.TriggerKeyword;
            TheResultingNode(triggerKeyword)
                .ShouldBePresent();

            var propertyKeyword = propertyTrigger.PropertyKeyword;
            TheResultingNode(propertyKeyword)
                .ShouldBePresent();

            var condition = propertyTrigger.Conditions[0];
            TheResultingNode(condition)
                .ShouldBePresent();

            var conditionPropertyName = condition.PropertyName;
            TheResultingNode(conditionPropertyName)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var conditionComparisonOperatorToken = condition.ComparisonOperatorToken;
            TheResultingNode(conditionComparisonOperatorToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.EqualsToken);

            var conditionValue = condition.PropertyValue;
            TheResultingNode(conditionValue)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Value == "bar baz");

            var qualifierToken = propertyTrigger.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.ImportantKeyword);

            var body = propertyTrigger.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSinglePropertyTrigger_WithMultipleConditions()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttrigger property foo = { bar baz }, abc <> { 123 456 }\r\n" +
                    "\t{ }\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var propertyTrigger = ruleSet.Body.Content[0] as UvssPropertyTriggerSyntax;
            TheResultingNode(propertyTrigger)
                .ShouldBePresent();

            var triggerKeyword = propertyTrigger.TriggerKeyword;
            TheResultingNode(triggerKeyword)
                .ShouldBePresent();

            var propertyKeyword = propertyTrigger.PropertyKeyword;
            TheResultingNode(propertyKeyword)
                .ShouldBePresent();

            var condition0 = propertyTrigger.Conditions[0];
            TheResultingNode(condition0)
                .ShouldBePresent();

            var condition0PropertyName = condition0.PropertyName;
            TheResultingNode(condition0PropertyName)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var condition0ComparisonOperatorToken = condition0.ComparisonOperatorToken;
            TheResultingNode(condition0ComparisonOperatorToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.EqualsToken);

            var condition0Value = condition0.PropertyValue;
            TheResultingNode(condition0Value)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Value == "bar baz");

            var condition1 = propertyTrigger.Conditions[1];
            TheResultingNode(condition1)
                .ShouldBePresent();

            var condition1PropertyName = condition1.PropertyName;
            TheResultingNode(condition1PropertyName)
                .ShouldBePresent()
                .ShouldHaveFullString("abc");
            
            var condition1ComparisonOperatorToken = condition1.ComparisonOperatorToken;
            TheResultingNode(condition1ComparisonOperatorToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.NotEqualsToken);

            var condition1Value = condition1.PropertyValue;
            TheResultingNode(condition1Value)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Value == "123 456");

            var body = propertyTrigger.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleEventTrigger()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttrigger event Mouse.enter\r\n" +
                    "\t{ }\r\n" +
                "}");
            
            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var eventTrigger = ruleSet.Body.Content[0] as UvssEventTriggerSyntax;
            TheResultingNode(eventTrigger)
                .ShouldBePresent();

            var triggerKeyword = eventTrigger.TriggerKeyword;
            TheResultingNode(triggerKeyword)
                .ShouldBePresent();

            var eventKeyword = eventTrigger.EventKeyword;
            TheResultingNode(eventKeyword)
                .ShouldBePresent();

            var eventName = eventTrigger.EventName;
            TheResultingNode(eventName)
                .ShouldBePresent()
                .ShouldHaveFullString("Mouse.enter");

            var argumentList = eventTrigger.ArgumentList;
            TheResultingNode(argumentList)
                .ShouldBeNull();

            var qualifierToken = eventTrigger.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBeNull();

            var body = eventTrigger.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleEventTrigger_WhenImportant()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttrigger event Mouse.enter !important\r\n" +
                    "\t{ }\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var eventTrigger = ruleSet.Body.Content[0] as UvssEventTriggerSyntax;
            TheResultingNode(eventTrigger)
                .ShouldBePresent();

            var triggerKeyword = eventTrigger.TriggerKeyword;
            TheResultingNode(triggerKeyword)
                .ShouldBePresent();

            var eventKeyword = eventTrigger.EventKeyword;
            TheResultingNode(eventKeyword)
                .ShouldBePresent();

            var eventName = eventTrigger.EventName;
            TheResultingNode(eventName)
                .ShouldBePresent()
                .ShouldHaveFullString("Mouse.enter");

            var argumentList = eventTrigger.ArgumentList;
            TheResultingNode(argumentList)
                .ShouldBeNull();

            var qualifierToken = eventTrigger.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.ImportantKeyword);

            var body = eventTrigger.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleEventTrigger_WithArgumentList()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttrigger event Mouse.enter (handled, set-handled)\r\n" +
                    "\t{ }\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var eventTrigger = ruleSet.Body.Content[0] as UvssEventTriggerSyntax;
            TheResultingNode(eventTrigger)
                .ShouldBePresent();

            var triggerKeyword = eventTrigger.TriggerKeyword;
            TheResultingNode(triggerKeyword)
                .ShouldBePresent();

            var eventKeyword = eventTrigger.EventKeyword;
            TheResultingNode(eventKeyword)
                .ShouldBePresent();

            var eventName = eventTrigger.EventName;
            TheResultingNode(eventName)
                .ShouldBePresent()
                .ShouldHaveFullString("Mouse.enter");

            var argumentList = eventTrigger.ArgumentList;
            TheResultingNode(argumentList)
                .ShouldBePresent();
            
            var arg0 = argumentList.ArgumentList[0] as SyntaxToken;
            TheResultingNode(arg0)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.HandledKeyword);

            var arg1 = argumentList.ArgumentList[1] as SyntaxToken;
            TheResultingNode(arg1)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.SetHandledKeyword);

            var qualifierToken = eventTrigger.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBeNull();

            var body = eventTrigger.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleEventTrigger_WithArgumentList_WhenImportant()
        {
            var document = UvssParser.Parse(
                "#foo\r\n" +
                "{\r\n" +
                    "\ttrigger event Mouse.enter (handled, set-handled) !important\r\n" +
                    "\t{ }\r\n" +
                "}");

            var ruleSet = document.Content[0] as UvssRuleSetSyntax;
            TheResultingNode(ruleSet)
                .ShouldBePresent();

            var eventTrigger = ruleSet.Body.Content[0] as UvssEventTriggerSyntax;
            TheResultingNode(eventTrigger)
                .ShouldBePresent();

            var triggerKeyword = eventTrigger.TriggerKeyword;
            TheResultingNode(triggerKeyword)
                .ShouldBePresent();

            var eventKeyword = eventTrigger.EventKeyword;
            TheResultingNode(eventKeyword)
                .ShouldBePresent();

            var eventName = eventTrigger.EventName;
            TheResultingNode(eventName)
                .ShouldBePresent()
                .ShouldHaveFullString("Mouse.enter");

            var argumentList = eventTrigger.ArgumentList;
            TheResultingNode(argumentList)
                .ShouldBePresent();

            var arg0 = argumentList.ArgumentList[0] as SyntaxToken;
            TheResultingNode(arg0)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.HandledKeyword);

            var arg1 = argumentList.ArgumentList[1] as SyntaxToken;
            TheResultingNode(arg1)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.SetHandledKeyword);

            var qualifierToken = eventTrigger.QualifierToken;
            TheResultingNode(qualifierToken)
                .ShouldBePresent()
                .ShouldBeOfKind(SyntaxKind.ImportantKeyword);

            var body = eventTrigger.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesStoryboard_WhenEmpty()
        {
            var document = UvssParser.Parse(
                "@foo {}");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            TheResultingNode(storyboard)
                .ShouldBePresent();

            var nameIdentifier = storyboard.NameIdentifier;
            TheResultingNode(nameIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var loopIdentifier = storyboard.LoopIdentifier;
            TheResultingNode(loopIdentifier)
                .ShouldBeNull();

            var body = storyboard.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesStoryboard_WhenEmpty_WithLoop()
        {
            var document = UvssParser.Parse(
                "@foo loop {}");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            TheResultingNode(storyboard)
                .ShouldBePresent();

            var nameIdentifier = storyboard.NameIdentifier;
            TheResultingNode(nameIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var loopIdentifier = storyboard.LoopIdentifier;
            TheResultingNode(loopIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("loop");

            var body = storyboard.Body;
            TheResultingNode(body)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesIncompleteStoryboard()
        {
            var document = UvssParser.Parse(
                "@foo");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            TheResultingNode(storyboard)
                .ShouldBePresent();

            var nameIdentifier = storyboard.NameIdentifier;
            TheResultingNode(nameIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var loopIdentifier = storyboard.LoopIdentifier;
            TheResultingNode(loopIdentifier)
                .ShouldBeNull();

            var body = storyboard.Body;
            TheResultingNode(body)
                .ShouldBeMissing();

            var openCurlyBrace = body.OpenCurlyBraceToken;
            TheResultingNode(openCurlyBrace)
                .ShouldBeMissing();

            var closeCurlyBrace = body.CloseCurlyBraceToken;
            TheResultingNode(closeCurlyBrace)
                .ShouldBeMissing();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesIncompleteStoryboard_WithOpenCurlyBrace()
        {
            var document = UvssParser.Parse(
                "@foo {");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            TheResultingNode(storyboard)
                .ShouldBePresent();

            var nameIdentifier = storyboard.NameIdentifier;
            TheResultingNode(nameIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var loopIdentifier = storyboard.LoopIdentifier;
            TheResultingNode(loopIdentifier)
                .ShouldBeNull();

            var body = storyboard.Body;
            TheResultingNode(body)
                .ShouldBePresent();

            var openCurlyBrace = body.OpenCurlyBraceToken;
            TheResultingNode(openCurlyBrace)
                .ShouldBePresent();

            var closeCurlyBrace = body.CloseCurlyBraceToken;
            TheResultingNode(closeCurlyBrace)
                .ShouldBeMissing();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesIncompleteStoryboard_WithCloseCurlyBrace()
        {
            var document = UvssParser.Parse(
                "@foo }");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            TheResultingNode(storyboard)
                .ShouldBePresent();

            var nameIdentifier = storyboard.NameIdentifier;
            TheResultingNode(nameIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("foo");

            var loopIdentifier = storyboard.LoopIdentifier;
            TheResultingNode(loopIdentifier)
                .ShouldBeNull();

            var body = storyboard.Body;
            TheResultingNode(body)
                .ShouldBePresent();

            var openCurlyBrace = body.OpenCurlyBraceToken;
            TheResultingNode(openCurlyBrace)
                .ShouldBeMissing();

            var closeCurlyBrace = body.CloseCurlyBraceToken;
            TheResultingNode(closeCurlyBrace)
                .ShouldBePresent();
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleStoryboardTarget()
        {
            var document = UvssParser.Parse(
                "@foo\r\n" +
                "{\r\n" +
                    "\ttarget {}\r\n" +
                "}");

            var target = ((UvssStoryboardSyntax)document.Content[0]).Body.Content[0] as UvssStoryboardTargetSyntax;
            TheResultingNode(target)
                .ShouldBePresent();

            var typeNameIdentifier = target.TypeNameIdentifier;
            TheResultingNode(typeNameIdentifier)
                .ShouldBeNull();

            var selector = target.Selector;
            TheResultingNode(selector)
                .ShouldBeNull();

            var body = target.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleStoryboardTarget_WithType_WithoutSelector()
        {
            var document = UvssParser.Parse(
                "@foo\r\n" +
                "{\r\n" +
                    "\ttarget Control {}\r\n" +
                "}");

            var target = ((UvssStoryboardSyntax)document.Content[0]).Body.Content[0] as UvssStoryboardTargetSyntax;
            TheResultingNode(target)
                .ShouldBePresent();

            var typeNameIdentifier = target.TypeNameIdentifier;
            TheResultingNode(typeNameIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("Control");

            var selector = target.Selector;
            TheResultingNode(selector)
                .ShouldBeNull();

            var body = target.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleStoryboardTarget_WithoutType_WithSelector()
        {
            var document = UvssParser.Parse(
                "@foo\r\n" +
                "{\r\n" +
                    "\ttarget (#foo #bar #baz) {}\r\n" +
                "}");

            var target = ((UvssStoryboardSyntax)document.Content[0]).Body.Content[0] as UvssStoryboardTargetSyntax;
            TheResultingNode(target)
                .ShouldBePresent();

            var typeNameIdentifier = target.TypeNameIdentifier;
            TheResultingNode(typeNameIdentifier)
                .ShouldBeNull();

            var selector = target.Selector.Selector;
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo #bar #baz");

            var body = target.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleStoryboardTarget_WithType_WithSelector()
        {
            var document = UvssParser.Parse(
                "@foo\r\n" +
                "{\r\n" +
                    "\ttarget Control (#foo #bar #baz) {}\r\n" +
                "}");

            var target = ((UvssStoryboardSyntax)document.Content[0]).Body.Content[0] as UvssStoryboardTargetSyntax;
            TheResultingNode(target)
                .ShouldBePresent();

            var typeNameIdentifier = target.TypeNameIdentifier;
            TheResultingNode(typeNameIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("Control");

            var selector = target.Selector.Selector;
            TheResultingNode(selector)
                .ShouldBePresent()
                .ShouldHaveFullString("#foo #bar #baz");

            var body = target.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleAnimation()
        {
            var document = UvssParser.Parse(
                "@foo\r\n" +
                "{\r\n" +
                    "\ttarget\r\n" +
                    "\t{\r\n" +
                        "\t\tanimation bar.baz {}\r\n" +
                    "\t}\r\n" +
                "}");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            var target = storyboard.Body.Content[0] as UvssStoryboardTargetSyntax;

            var animation = target.Body.Content[0] as UvssAnimationSyntax;
            TheResultingNode(animation)
                .ShouldBePresent();

            var animationKeyword = animation.AnimationKeyword;
            TheResultingNode(animationKeyword)
                .ShouldBePresent();

            var propertyName = animation.PropertyName;
            TheResultingNode(propertyName)
                .ShouldBePresent()
                .ShouldHaveFullString("bar.baz");

            var navigationExpression = animation.NavigationExpression;
            TheResultingNode(navigationExpression)
                .ShouldBeNull();

            var body = animation.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleAnimation_WithNavigationExpression()
        {
            var document = UvssParser.Parse(
                "@foo\r\n" +
                "{\r\n" +
                    "\ttarget\r\n" +
                    "\t{\r\n" +
                        "\t\tanimation bar.baz | something as SomeType {}\r\n" +
                    "\t}\r\n" +
                "}");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            var target = storyboard.Body.Content[0] as UvssStoryboardTargetSyntax;

            var animation = target.Body.Content[0] as UvssAnimationSyntax;
            TheResultingNode(animation)
                .ShouldBePresent();

            var animationKeyword = animation.AnimationKeyword;
            TheResultingNode(animationKeyword)
                .ShouldBePresent();

            var propertyName = animation.PropertyName;
            TheResultingNode(propertyName)
                .ShouldBePresent()
                .ShouldHaveFullString("bar.baz");

            var navigationExpression = animation.NavigationExpression;
            TheResultingNode(navigationExpression)
                .ShouldBePresent()
                .ShouldHaveFullString("| something as SomeType");

            var body = animation.Body;
            TheResultingNode(body)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Content.Count == 0);
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleAnimationKeyframe()
        {
            var document = UvssParser.Parse(
                "@foo\r\n" +
                "{\r\n" +
                    "\ttarget\r\n" +
                    "\t{\r\n" +
                        "\t\tanimation bar\r\n" +
                        "\t\t{\t\n" +
                            "\t\t\tkeyframe 0 { baz }\r\n" +
                        "\t\t}\r\n" +
                    "\t}\r\n" +
                "}");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            var target = storyboard.Body.Content[0] as UvssStoryboardTargetSyntax;
            var animation = target.Body.Content[0] as UvssAnimationSyntax;

            var keyframe = animation.Body.Content[0] as UvssAnimationKeyframeSyntax;
            TheResultingNode(keyframe)
                .ShouldBePresent();

            var keyframeKeyword = keyframe.KeyframeKeyword;
            TheResultingNode(keyframeKeyword)
                .ShouldBePresent();

            var timeToken = keyframe.TimeToken;
            TheResultingNode(timeToken)
                .ShouldBePresent()
                .ShouldHaveFullString("0");

            var easingIdentifier = keyframe.EasingIdentifier;
            TheResultingNode(easingIdentifier)
                .ShouldBeNull();

            var value = keyframe.Value;
            TheResultingNode(value)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Value == "baz");
        }

        [TestMethod]
        public void UvssParser_CorrectlyParsesSingleAnimationKeyframe_WithEasing()
        {
            var document = UvssParser.Parse(
                "@foo\r\n" +
                "{\r\n" +
                    "\ttarget\r\n" +
                    "\t{\r\n" +
                        "\t\tanimation bar\r\n" +
                        "\t\t{\t\n" +
                            "\t\t\tkeyframe 0 ease-out-linear { baz }\r\n" +
                        "\t\t}\r\n" +
                    "\t}\r\n" +
                "}");

            var storyboard = document.Content[0] as UvssStoryboardSyntax;
            var target = storyboard.Body.Content[0] as UvssStoryboardTargetSyntax;
            var animation = target.Body.Content[0] as UvssAnimationSyntax;

            var keyframe = animation.Body.Content[0] as UvssAnimationKeyframeSyntax;
            TheResultingNode(keyframe)
                .ShouldBePresent();

            var keyframeKeyword = keyframe.KeyframeKeyword;
            TheResultingNode(keyframeKeyword)
                .ShouldBePresent();

            var timeToken = keyframe.TimeToken;
            TheResultingNode(timeToken)
                .ShouldBePresent()
                .ShouldHaveFullString("0");

            var easingIdentifier = keyframe.EasingIdentifier;
            TheResultingNode(easingIdentifier)
                .ShouldBePresent()
                .ShouldHaveFullString("ease-out-linear");

            var value = keyframe.Value;
            TheResultingNode(value)
                .ShouldBePresent()
                .ShouldSatisfyTheCondition(x => x.Value == "baz");
        }
    }
}