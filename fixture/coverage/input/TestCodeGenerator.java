// Copyright 2011-2013 The PageObjectGenerator Authors.
// Copyright 2011 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS-IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package com.google.testing.pogen.generator.test.java;

import org.apache.commons.lang3.StringUtils;

import com.google.common.base.Preconditions;
import com.google.common.collect.HashMultiset;
import com.google.testing.pogen.generator.test.PageObjectUpdateException;
import com.google.testing.pogen.parser.template.HtmlTagInfo;
import com.google.testing.pogen.parser.template.TemplateInfo;
import com.google.testing.pogen.parser.template.VariableInfo;

/**
 * A class to generate skeleton test code designed by PageObject pattern for Selenium2 (WebDriver)
 * from the specified {@link TemplateInfo} instance.
 * 
 * @author Kazunori Sakamoto
 */
public abstract class TestCodeGenerator {
  /**
   * A string to indicate the start of generated fields and getter methods.
   */
  public static final String GENERATED_CODE_END_MARK =
      "/* -------------------- GENERATED CODE END -------------------- */";
  /**
   * A string to indicate the end of generated fields and getter methods.
   */
  public static final String GENERATED_CODE_START_MARK =
      "/* ------------------- GENERATED CODE START ------------------- */";
  /**
   * An indent string.
   */
  private final String indent;
  /**
   * A new-line string.
   */
  private final String newLine;

  /**
   * Constructs an instance with the default indent and new-line strings.
   */
  public TestCodeGenerator() {
    this("  ", "\n");
  }

  /**
   * Constructs an instance with the specified indent and the specified new-line strings.
   * 
   * @param indent the string of indent for generating source code
   * @param newLine the string of new line for generating source code
   */
  public TestCodeGenerator(String indent, String newLine) {
    this.indent = indent;
    this.newLine = newLine;
  }

  /**
   * Generates skeleton test code with getter methods for html tags, texts and attributes to
   * retrieve values of variables from Selenium2.
   * 
   * @param templateInfo the {@link TemplateInfo} of the template whose skeleton test code we want
   *        to generate
   * @param packageName the package name to generate skeleton test code
   * @param className the class name to generate skeleton test code
   * @return the generated skeleton test code
   */
  public String generate(TemplateInfo templateInfo, String packageName, String className) {
    Preconditions.checkNotNull(templateInfo);
    Preconditions.checkNotNull(packageName);
    Preconditions.checkNotNull(className);

    StringBuilder builder = new StringBuilder();
    appendLine(builder, 0, String.format("package %s;", packageName));
    appendLine(builder);

    appendLine(builder, 0, "import static org.junit.Assert.*;");
    appendLine(builder, 0, "import static org.hamcrest.Matchers.*;");
    appendLine(builder);

    appendLine(builder, 0, "import org.openqa.selenium.By;");
    appendLine(builder, 0, "import org.openqa.selenium.WebDriver;");
    appendLine(builder, 0, "import org.openqa.selenium.WebElement;");
    appendLine(builder, 0, "import org.openqa.selenium.support.FindBy;");
    appendLine(builder, 0, "import org.openqa.selenium.support.How;");
    appendLine(builder);
    appendLine(builder, 0, "import java.util.ArrayList;");
    appendLine(builder, 0, "import java.util.HashMap;");
    appendLine(builder, 0, "import java.util.List;");
    appendLine(builder, 0, "import java.util.regex.Matcher;");
    appendLine(builder, 0, "import java.util.regex.Pattern;");
    appendLine(builder);

    appendLine(builder, 0, String.format("public class %sPage extends AbstractPage {", className));
    appendLine(builder, 1, String.format("public %sPage(WebDriver driver) {", className));
    appendLine(builder, 2, "super(driver);");
    appendLine(builder, 2, "assertInvariant();");
    appendLine(builder, 1, "}");
    appendLine(builder);

    appendLine(builder, 1, "private void assertInvariant() {");
    appendLine(builder, 1, "}");
    appendLine(builder);

    appendLine(builder, 1, GENERATED_CODE_START_MARK);
    appendFieldsAndGetters(builder, templateInfo);
    appendLine(builder, 1, GENERATED_CODE_END_MARK);
    appendLine(builder, 0, "}");
    return builder.toString();
  }

  /**
   * Updates existing test code with getter methods for html tags, texts and attributes to retrieve
   * the values of the variables from Selenium2.
   * 
   * @param templateInfo the {@link TemplateInfo} of the template whose skeleton test code we want
   *        to generate
   * @param code the existing test code
   * @return the updated skeleton test code
   * @throws PageObjectUpdateException if the existing test code doesn't have generated code
   */
  public String update(TemplateInfo templateInfo, String code) throws PageObjectUpdateException {
    Preconditions.checkNotNull(templateInfo);
    Preconditions.checkNotNull(code);

    StringBuilder builder = new StringBuilder();
    int startIndex = code.indexOf(GENERATED_CODE_START_MARK);
    int endIndex = code.indexOf(GENERATED_CODE_END_MARK);
    if (startIndex < 0 || endIndex < 0 || endIndex < startIndex) {
      throw new PageObjectUpdateException();
    }
    builder.append(code.subSequence(0, startIndex + GENERATED_CODE_START_MARK.length()));
    builder.append(newLine);
    appendFieldsAndGetters(builder, templateInfo);
    builder.append(code.subSequence(endIndex, code.length()));
    return builder.toString();
  }

  /**
   * Appends the body of skeleton test code, that is, only html tag fields and getter methods to
   * retrieve the values of the variables into the given string builder.
   * 
   * @param builder {@link StringBuilder} the generated test code will be appended to
   * @param templateInfo the {@link TemplateInfo} of the template whose skeleton test code we want
   *        to generate
   */
  private void appendFieldsAndGetters(StringBuilder builder, TemplateInfo templateInfo) {
    // Create new StringBuilder to separate methods group from fields group such
    // as "private int field1; private int field2;
    // private void method1() {} private void method2() {}".
    StringBuilder methodBuilder = new StringBuilder();
    HashMultiset<String> varNameCounter = HashMultiset.create();

    appendLine(
        builder,
        1,
        "private static Pattern commentPattern = Pattern.compile(\"<!--POGEN,([^,]*),([^,]*),(.*?)-->\", Pattern.DOTALL);");

    for (HtmlTagInfo tagInfo : templateInfo.getHtmlTagInfos()) {
      // Skip this variable if it has no parent html tag
      // TODO(kazuu): Deal with these variable with a more proper way
      if (!tagInfo.hasParentTag()) {
        continue;
      }

      String attrValue = tagInfo.getAttributeValue();
      boolean isRepeated = tagInfo.isRepeated();

      for (VariableInfo varInfo : tagInfo.getVariableInfos()) {
        // When the same template variable appears in other html tags,
        // varIndex > 1 is satisfied
        varNameCounter.add(varInfo.getName());
        int varIndex = varNameCounter.count(varInfo.getName());
        String newVarName = varInfo.getName() + convertToString(varIndex);

        appendElementGetter(builder, methodBuilder, newVarName, attrValue, isRepeated);
        if (!varInfo.isManipulableTag()) {
          appendTextGetter(methodBuilder, newVarName, tagInfo, varInfo, isRepeated);
        }
        for (String attrName : varInfo.getSortedAttributeNames()) {
          appendAttributeGetter(methodBuilder, newVarName, attrName, attrValue, isRepeated);
        }
      }
    }
    // Append method definitions after field definitions
    builder.append(methodBuilder);
  }

  /**
   * Appends a getter method and also a field if needed for the html tag which contains the variable
   * specified by the name into the given string builder.
   * 
   * @param fieldBuilder {@link StringBuilder} the generated field will be appended to
   * @param methodBuilder {@link StringBuilder} the generated method will be appended to
   * @param variableName the variable name
   * @param assignedAttributeValue the attribute value assigned to the html tag
   * @param isRepeated the boolean whether the specified html tag appears in a repeated part
   */
  private void appendElementGetter(StringBuilder fieldBuilder, StringBuilder methodBuilder,
      String variableName, String assignedAttributeValue, boolean isRepeated) {
    if (!isRepeated) {
      appendField(fieldBuilder, variableName, assignedAttributeValue);
      appendGetter(methodBuilder, variableName, "", "WebElement", "ElementOf");
    } else {
      appendListGetter(methodBuilder, variableName, "", "WebElement", "ElementsOf",
          assignedAttributeValue);
    }
  }

  /**
   * Appends a getter method for the text of html tag which contains the variable specified by the
   * name into the given string builder.
   * 
   * @param methodBuilder {@link StringBuilder} the generated method will be appended to
   * @param uniqueVariableName the unique variable name with a sequential number
   * @param tagInfo the information of the html tag containing the template variable
   * @param varInfo the information of the template variable
   * @param isRepeated the boolean whether the specified html tag appears in a repeated part
   */
  private void appendTextGetter(StringBuilder methodBuilder, String uniqueVariableName,
      HtmlTagInfo tagInfo, VariableInfo varInfo, boolean isRepeated) {
    // TODO(kazuu): Help to select proper one from getFoo, getFoo2, getFoo3 ...
    appendLine(methodBuilder);
    String methodNamePrefix = !isRepeated ? "String getTextOf" : "List<String> getTextsOf";
    String foundedProcess =
        !isRepeated ? "return matcher.group(3);" : "result.add(matcher.group(3));";
    String finalProcess = !isRepeated ? "return null;" : "return result;";
    appendLine(
        methodBuilder,
        1,
        String.format("public %s%s() {", methodNamePrefix,
            StringUtils.capitalize(uniqueVariableName)));
    if (isRepeated) {
      appendLine(methodBuilder, 2, "List<String> result = new ArrayList<String>();");
    }
    appendLine(methodBuilder, 2,
        "Matcher matcher = commentPattern.matcher(driver.getPageSource());");
    appendLine(methodBuilder, 2, "while (matcher.find()) {");
    appendLine(methodBuilder, 3, String.format(
        "if (matcher.group(1).equals(\"%s\") && matcher.group(2).equals(\"%s\")) {",
        tagInfo.getAttributeValue(), varInfo.getName()));
    appendLine(methodBuilder, 4, foundedProcess);
    appendLine(methodBuilder, 3, "}");
    appendLine(methodBuilder, 2, "}");
    appendLine(methodBuilder, 2, finalProcess);
    appendLine(methodBuilder, 1, "}");
  }

  /**
   * Appends a getter method for the attribute of html tag which contains the variable specified by
   * the name into the given string builder.
   * 
   * @param methodBuilder {@link StringBuilder} the generated method will be appended to
   * @param variableName the variable name
   * @param attributeName the name of the attribute which contains template variables
   * @param assignedAttributeValue the attribute value assigned to the html tag
   * @param isRepeated the boolean whether the specified html tag appears in a repeated part
   */
  private void appendAttributeGetter(StringBuilder methodBuilder, String variableName,
      String attributeName, String assignedAttributeValue, boolean isRepeated) {
    if (!isRepeated) {
      appendGetter(methodBuilder, variableName, ".getAttribute(\"" + attributeName + "\")",
          "String", "AttributeOf" + StringUtils.capitalize(attributeName) + "On");
    } else {
      appendListGetter(methodBuilder, variableName, ".getAttribute(\"" + attributeName + "\")",
          "String", "AttributesOf" + StringUtils.capitalize(attributeName) + "On",
          assignedAttributeValue);
    }
  }

  /**
   * Appends a private field for accessing the html tag which has the specified attribute value and
   * contains the variable specified by the name with {@literal @FindBy(how = How.XPATH , ...)} into
   * the given string builder.
   * 
   * @param builder {@link StringBuilder} the generated test code will be appended to
   * @param variableName the variable name
   * @param assignedAttributeValue the attribute value assigned to the html tag
   */
  protected abstract void appendField(StringBuilder builder, String variableName,
      String assignedAttributeValue);

  /**
   * Appends a getter method for the variable specified by the name or the result of invoking the
   * method described by the given prefix on the variable into the given string builder.
   * 
   * @param builder {@link StringBuilder} the generated test code will be appended to
   * @param variableName the variable name
   * @param elementSuffixForInvoking the suffix of the {@code WebElement} variable that specifies a
   *        method name with a dot to invoke it, e.g. {@literal".getText()"}, or an empty string to
   *        access the variable directly.
   * @param returnType the return type of the generated getter method
   * @param methodNamePrefix the name prefix of the generated method
   */
  private void appendGetter(StringBuilder builder, String variableName,
      String elementSuffixForInvoking, String returnType, String methodNamePrefix) {
    appendLine(builder);
    // TODO(kazuu): Help to select proper one from getFoo, getFoo2, getFoo3 ...
    appendLine(
        builder,
        1,
        String.format("public %s get%s%s() {", returnType, methodNamePrefix,
            StringUtils.capitalize(variableName)));
    appendLine(builder, 2, String.format("return %s%s;", variableName, elementSuffixForInvoking));
    appendLine(builder, 1, "}");
  }

  /**
   * Appends a getter method for the list of the variables specified by the name or the result of
   * invoking the method described by the given prefix on the variable. The generated getter method
   * is used for repeated part in templates such as "{foreach $x in $xs}{@literal <div>$x</div>}
   * {/foreach}"
   * 
   * @param builder {@link StringBuilder} the generated test code will be appended to
   * @param variableName the variable name
   * @param elementSuffixForInvoking the suffix of the {@code WebElement} variable that specifies a
   *        method name with a dot to invoke it, e.g. {@literal ".getText()"}, or an empty string to
   *        access the variable directly.
   * @param returnType the return type of the generated getter method
   * @param methodNamePrefix the name prefix of the generated method
   * @param assignedAttributeValue the attribute value assigned to the html tag
   */
  protected abstract void appendListGetter(StringBuilder builder, String variableName,
      String elementSuffixForInvoking, String returnType, String methodNamePrefix,
      String assignedAttributeValue);

  /**
   * Converts the specified number to a string. Results an empty string if the number is 1.
   * 
   * @param number the number to be converted
   * 
   * @return an empty string if the specified number is 1, otherwise prefix + number
   */
  private String convertToString(int number) {
    return number == 1 ? "" : String.valueOf(number);
  }

  /**
   * Appends a new-line character into the specified builder.
   * 
   * @param builder the string builder to be appended a new-line character
   */
  protected void appendLine(StringBuilder builder) {
    builder.append(newLine);
  }

  /**
   * Appends the specified line with the specified number of indent string and a new-line character
   * into the specified builder.
   * 
   * @param builder the string builder to be appended strings
   * @param indentCount the number of indent string to be appended at the beginning of the line
   * @param line the line string to be be appended into the builder
   */
  protected void appendLine(StringBuilder builder, int indentCount, String line) {
    for (int i = 0; i < indentCount; i++) {
      builder.append(indent);
    }
    builder.append(line);
    builder.append(newLine);
  }
}
