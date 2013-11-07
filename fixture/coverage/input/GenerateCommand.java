// Copyright 2011 The PageObjectGenerator Authors.
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

package com.google.testing.pogen;

import java.io.File;
import java.io.IOException;
import java.net.URI;
import java.net.URL;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.Arrays;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.filefilter.FileFilterUtils;
import org.apache.commons.io.filefilter.RegexFileFilter;

import com.google.common.base.Preconditions;
import com.google.common.base.Strings;
import com.google.common.io.Files;
import com.google.common.io.Resources;
import com.google.testing.pogen.generator.template.TemplateUpdater;
import com.google.testing.pogen.generator.template.TemplateUpdaters;
import com.google.testing.pogen.generator.test.PageObjectUpdateException;
import com.google.testing.pogen.generator.test.java.NameConverter;
import com.google.testing.pogen.generator.test.java.TestCodeGenerator;
import com.google.testing.pogen.generator.test.java.TestCodeGenerators;
import com.google.testing.pogen.parser.template.TemplateInfo;
import com.google.testing.pogen.parser.template.TemplateParseException;
import com.google.testing.pogen.parser.template.TemplateParser;
import com.google.testing.pogen.parser.template.TemplateParsers;
import com.google.testing.pogen.parser.template.jsf.JsfParser;

/**
 * A class which represents the generate command to generate modified templates and skeleton test
 * code.
 * 
 * @author Kazunori Sakamoto
 */
public class GenerateCommand extends Command {

  /**
   * A file name of the {@code AbstractPage} class.
   */
  private static final String ABSTRACT_PAGE_NAME = "AbstractPage.java";
  /**
   * A package name of the {@code AbstractPage} class.
   */
  private static final String ABSTRACT_PAGE_PACKAGE =
      "com.google.testing.pogen.generator.test.java.page";

  /**
   * Template paths to be parsed.
   */
  private final String[] templatePaths;
  /**
   * A output directory path of test codes.
   */
  private final String testOutDirPath;
  /**
   * A package name to generate skeleton test codes.
   */
  private final String packageName;
  /**
   * A boolean whether prints processed files verbosely.
   */
  private final boolean verbose;
  /**
   * A name of the attribute to be assigned for tags containing template variables.
   */
  private final String attributeName;
  /**
   * A path of root directory which contains html template files.
   */
  private String rootDirectoryPath;
  /**
   * A pattern for finding template files in the root directory.
   */
  private String templateFilePattern;
  /**
   * A boolean whether template files are recursively found.
   */
  private boolean isRecusive;

  /**
   * Constructs an instance with the the specified template paths, specified output-directory path,
   * the specified package name, the specified attribute name and the boolean of the verbose mode.
   * 
   * @param templatePaths the template paths to be parsed
   * @param testOutDirPath the output directory path of test codes
   * @param packageName the package name to generate skeleton test codes
   * @param attributeName the name of the attribute to be assigned for tags containing template
   *        variables
   * @param verbose the boolean whether prints processed files verbosely
   * @param rootDirectoryPath the root directory of html template files
   * @param templateFilePattern the pattern for finding template files in the root directory
   * @param isRecusive the boolean whether template files are recursively found
   */
  public GenerateCommand(String[] templatePaths, String testOutDirPath, String packageName,
      String attributeName, boolean verbose, String rootDirectoryPath, String templateFilePattern,
      boolean isRecusive) {
    this.templatePaths = Arrays.copyOf(templatePaths, templatePaths.length);
    this.testOutDirPath = testOutDirPath;
    this.packageName = packageName;
    this.attributeName = attributeName;
    this.verbose = verbose;
    this.rootDirectoryPath = rootDirectoryPath;
    this.templateFilePattern = templateFilePattern;
    this.isRecusive = isRecusive;
  }

  @Override
  public void execute() throws IOException {
    File rootInputDir = createDirectory(rootDirectoryPath, false, true);
    File testOutDir = createDirectory(testOutDirPath, false, true);

    // Check whether the root directory exists
    if (!rootInputDir.exists()) {
      throw new FileProcessException("Not found root intpu directory", rootInputDir);
    }

    // Generate the AbstractPage class
    File newAbstractPageFile = new File(testOutDir.getPath(), ABSTRACT_PAGE_NAME);
    if (!newAbstractPageFile.exists()) {
      URL abstractPageUrl = Resources.getResource(ABSTRACT_PAGE_NAME);
      String abstractPage = Resources.toString(abstractPageUrl, Charset.defaultCharset());
      abstractPage = abstractPage.replaceAll(ABSTRACT_PAGE_PACKAGE, packageName);
      Files.write(abstractPage, newAbstractPageFile, Charset.defaultCharset());
    } else if (verbose) {
      System.err.println("Already exists: " + newAbstractPageFile.getAbsolutePath() + ".");
    }

    // Collect the template files from the arguments indicating paths of template files
    ArrayList<File> templateFiles = new ArrayList<File>();
    for (String templatePath : templatePaths) {
      File file = createFileFromFilePath(templatePath);
      templateFiles.add(file);
    }

    // Collect the template files from the specified pattern with the root directory
    if (!Strings.isNullOrEmpty(templateFilePattern)) {
      templateFiles.addAll(FileUtils.listFiles(rootInputDir, new RegexFileFilter(
          templateFilePattern), isRecusive ? FileFilterUtils.trueFileFilter() : null));
    }

    TemplateUpdater updater = TemplateUpdaters.getPreferredUpdater(attributeName);
    TestCodeGenerator generator = TestCodeGenerators.getPreferredGenerator(attributeName);
    for (File file : templateFiles) {
      checkExistenceAndPermission(file, true, true);
      try {
        TemplateParser parser = TemplateParsers.getPreferredParser(file.getPath(), attributeName);
        if (attributeName.equals("id") && parser instanceof JsfParser) {
          System.out
              .println("WARNING: Using id attribute is not recommmended for JSF templat engine.");
        }
        parseAndGenerate(file, rootInputDir, testOutDir, parser, updater, generator);
      } catch (TemplateParseException e) {
        throw new FileProcessException("Errors occur in parsing the specified files", file, e);
      } catch (PageObjectUpdateException e) {
        throw new FileProcessException("Errors occur in updating the specified files", file, e);
      }
    }
  }

  /**
   * Parses the specified template file and generates a modified template file and skeleton test
   * code.
   * 
   * @param templateFile the template file to be modified
   * @param rootInputDir the root input directory of template files
   * @param codeOutDir the output directory of skeleton test code
   * @param parser the parser to parse template files
   * @param updater the updater to update template files
   * @param generator the generator to generate skeleton test code
   * @throws IOException if errors occur in reading and writing files
   * @throws TemplateParseException if the specified template is in bad format
   * @throws PageObjectUpdateException if the existing test code doesn't have generated code
   */
  private void parseAndGenerate(File templateFile, File rootInputDir, File codeOutDir,
      TemplateParser parser, TemplateUpdater updater, TestCodeGenerator generator)
      throws IOException, TemplateParseException, PageObjectUpdateException {
    Preconditions.checkNotNull(templateFile);
    Preconditions.checkNotNull(rootInputDir);
    Preconditions.checkNotNull(codeOutDir);
    Preconditions.checkArgument(!Strings.isNullOrEmpty(packageName));
    Preconditions.checkNotNull(parser);

    if (verbose) {
      System.out.println(templateFile.getAbsolutePath() + " ... ");
    }
    File orgTemplateFile = backupFile(templateFile);
    String pageName = NameConverter.getJavaClassName(getFileNameWithoutExtension(templateFile));
    // Read template file
    String template = Files.toString(orgTemplateFile, Charset.defaultCharset());
    // Parse template extracting template variables
    TemplateInfo templateInfo = parser.parse(template);
    if (verbose) {
      System.out.print(".");
    }
    // Generate modified template
    String modifiedTemplate = updater.generate(templateInfo);
    if (verbose) {
      System.out.print(".");
    }

    // Construct path of skeleton test code
    URI relativeDirUri = rootInputDir.toURI().relativize(templateFile.getParentFile().toURI());
    String relativeDirPath = relativeDirUri.toString();
    if (relativeDirPath.endsWith("/")) {
      relativeDirPath = relativeDirPath.substring(0, relativeDirPath.length() - 1);
    }
    if (!Strings.isNullOrEmpty(relativeDirPath)) {
      relativeDirPath = File.separatorChar + relativeDirPath;
    }
    String packagePrefix = relativeDirPath.replace('/', '.');
    File actualDir = new File(codeOutDir.getPath() + relativeDirPath);
    actualDir.mkdirs();

    // Generate skeleton test code
    File codeFile = new File(actualDir, pageName + "Page.java");
    if (codeFile.exists() && !codeFile.canWrite()) {
      throw new FileProcessException("No permission for writing the specified file", codeFile);
    }

    // @formatter:off
    String testCode = codeFile.exists()
        ? generator.update(templateInfo, Files.toString(codeFile, Charset.defaultCharset()))
        : generator.generate(templateInfo, packageName + packagePrefix, pageName);
    // @formatter:on
    if (verbose) {
      System.out.print(".");
    }
    // Write generated template and skeleton test code
    Files.write(modifiedTemplate, templateFile, Charset.defaultCharset());
    if (verbose) {
      System.out.print(".");
    }
    Files.write(testCode, codeFile, Charset.defaultCharset());
    if (verbose) {
      System.out.println("\n" + templateFile.getAbsolutePath() + " processed successfully");
    }
  }

  /**
   * Backups the specified file. If the backup file has existed, does nothing.
   * 
   * @param file the file to be backuped
   * @return newly backuped file whose name is xxxxx.org if the backup file doesn't exist, otherwise
   *         existing backup file
   * @throws IOException if errors occur in backuping files
   */
  private File backupFile(File file) throws IOException {
    Preconditions.checkNotNull(file);

    File orgHtmlFile = new File(file.getPath() + ".org");
    if (!orgHtmlFile.exists()) {
      Files.copy(file, orgHtmlFile);
    }
    return orgHtmlFile;
  }

  /**
   * Gets the file name without the extension of the specified file.
   * 
   * @param file file to get the name
   * @return the file name without the extension
   */
  private String getFileNameWithoutExtension(File file) {
    Preconditions.checkNotNull(file);

    String fileName = file.getName();
    int endIndex = fileName.lastIndexOf('.');
    if (endIndex > 0) {
      return fileName.substring(0, endIndex);
    }
    return fileName;
  }
}
