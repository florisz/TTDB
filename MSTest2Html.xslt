<?xml version="1.0" encoding="utf-8"?> 
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:vs="http://microsoft.com/schemas/VisualStudio/TeamTest/2006">

  <xsl:output method="html"/>
  
  <xsl:template match="/"> 
    <html> 
      <body style="font-family:Verdana; font-size:10pt"> 
        <h1>Test Results Summary</h1> 
        <table style="font-family:Verdana; font-size:10pt"> 
          <tr> 
            <td> 
              <b>Run Date/Time</b> 
            </td> 
            <td> 
              <xsl:value-of select="//vs:Times/@creation"/> 
            </td> 
          </tr> 
          <tr> 
            <td> 
              <b>Results </b> 
            </td> 
            <td> 
              <xsl:value-of select="//vs:Deployment/@runDeploymentRoot"/> 
            </td> 
          </tr> 
        </table> 
        <xsl:call-template name="summary" /> 
        <xsl:apply-templates select="//vs:Results" /> 
      </body> 
    </html> 
  </xsl:template> 
  
  <xsl:template name="summary"> 
    <h3>Test Summary</h3> 
    <table style="width:640;border:1px solid black;font-family:Verdana; font-size:10pt"> 
      <tr> 
        <td style="font-weight:bold">Total</td> 
        <td style="font-weight:bold">Failed</td> 
        <td style="font-weight:bold">Passed</td> 
      </tr> 
      <tr> 
        <td> 
          <xsl:value-of select="//vs:ResultSummary/vs:Counters/@total"/> 
        </td> 
        <td style="background-color:pink;"> 
          <xsl:value-of select="//vs:ResultSummary/vs:Counters/@failed"/> 
        </td> 
        <td style="background-color:lightgreen;"> 
          <xsl:value-of select="//vs:ResultSummary/vs:Counters/@passed"/> 
        </td> 
      </tr> 
    </table> 
  </xsl:template> 
  
  <xsl:template match="vs:Results"> 
    <a name="Unit Test Results">
      <h3>Unit Test Results</h3> 
    </a>
    <table style="width:640;border:1px solid black;font-family:Verdana; font-size:10pt;"> 
      <tr> 
        <td style="font-weight:bold">Test Name</td> 
        <td style="font-weight:bold">Result</td> 
      </tr> 
      <xsl:apply-templates select="vs:UnitTestResult" mode="result"/> 
    </table>
    
    <h3>Unit Test Details</h3> 
    <xsl:apply-templates select="vs:UnitTestResult" mode="details"/> 
  </xsl:template> 
  
  <xsl:template match="vs:UnitTestResult" mode="result"> 
    <tr> 
      <xsl:attribute name="style"> 
        <xsl:choose> 
          <xsl:when test="@outcome = 'Failed'">background-color:pink;</xsl:when> 
          <xsl:when test="@outcome = 'Passed'">background-color:lightgreen;</xsl:when> 
          <xsl:otherwise>background-color:yellow;</xsl:otherwise> 
        </xsl:choose> 
      </xsl:attribute> 
      <td>
        <xsl:variable name="testId">
          <xsl:value-of select="@testId"/>
        </xsl:variable>
        <xsl:variable name="testReference">
          <xsl:value-of select="substring-before(//vs:TestDefinitions/vs:UnitTest[@id=$testId]/vs:TestMethod/@className, ',')"/>
          <xsl:text>.</xsl:text>
          <xsl:value-of select="@testName"/>
        </xsl:variable>
        <a>
          <xsl:attribute name="name">
            <xsl:value-of select="$testReference"/>
            <xsl:text>.Result</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="href">
            <xsl:text>#</xsl:text>
            <xsl:value-of select="$testReference"/>
            <xsl:text>.Details</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="$testReference"/>
        </a>
      </td> 
      <td> 
        <xsl:choose> 
          <xsl:when test="@outcome = 'Failed'">FAILED</xsl:when> 
          <xsl:when test="@outcome = 'Passed'">Passed</xsl:when> 
          <xsl:otherwise>Inconclusive</xsl:otherwise> 
        </xsl:choose> 
      </td> 
    </tr> 
  </xsl:template> 
  
  <xsl:template match="vs:UnitTestResult" mode="details"> 
    <xsl:variable name="testId">
      <xsl:value-of select="@testId"/>
    </xsl:variable>
    <xsl:variable name="testReference">
      <xsl:value-of select="substring-before(//vs:TestDefinitions/vs:UnitTest[@id=$testId]/vs:TestMethod/@className, ',')"/>
      <xsl:text>.</xsl:text>
      <xsl:value-of select="@testName"/>
    </xsl:variable>
    <table style="width:640;border:1px solid black;font-family:Verdana; font-size:10pt;">
      <tr>
        <td>
          <h5>
            <a>
              <xsl:attribute name="name">
                <xsl:value-of select="$testReference"/>
                <xsl:text>.Details</xsl:text>
              </xsl:attribute>
              <xsl:value-of select="$testReference"/>
            </a>
          </h5> 
        </td>
      </tr>
      <tr>
        <td>
          <xsl:attribute name="style"> 
            <xsl:choose> 
              <xsl:when test="@outcome = 'Failed'">background-color:pink;</xsl:when> 
              <xsl:when test="@outcome = 'Passed'">background-color:lightgreen;</xsl:when> 
              <xsl:otherwise>background-color:yellow;</xsl:otherwise> 
            </xsl:choose> 
          </xsl:attribute> 
          <xsl:choose> 
            <xsl:when test="@outcome = 'Failed'">FAILED</xsl:when> 
            <xsl:when test="@outcome = 'Passed'">Passed</xsl:when> 
            <xsl:otherwise>Inconclusive</xsl:otherwise> 
          </xsl:choose> 
        </td>
      </tr>
      <xsl:if test="vs:Output/vs:StdOut">
        <tr>
          <td>
             <h5>Output</h5>
          </td>
        </tr>
        <tr>
          <td>
             <pre>
               <xsl:value-of select="vs:Output/vs:StdOut"/>
             </pre>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="vs:Output/vs:ErrorInfo">
        <tr>
          <td>
             <h5>ErrorInfo</h5>
          </td>
        </tr>
        <tr>
          <td>
             <pre>
               <xsl:value-of select="vs:Output/vs:ErrorInfo"/>
             </pre>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <td>
          <a>
            <xsl:attribute name="href">
              <xsl:text>#</xsl:text>
              <xsl:value-of select="$testReference"/>
              <xsl:text>.Result</xsl:text>
            </xsl:attribute>
            <xsl:text>Back</xsl:text>
          </a>
        </td>
      </tr>
    </table>
    <p/>
  </xsl:template>

</xsl:stylesheet>