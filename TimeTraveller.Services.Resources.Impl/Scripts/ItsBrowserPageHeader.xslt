<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:fn="http://www.w3.org/2005/02/xpath-functions"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" />
  <xsl:template name="DateTime">

    <script type="text/javascript">
      var d = new Date();
      document.write(d.toLocaleDateString());
      document.write(" ");
      document.write(d.toLocaleTimeString());
    </script>
  </xsl:template>
  
  <xsl:template name="PageHeader">
    <table width="100%">
      <thead>
        <h1>
          Information Timeline Browser
        </h1>
      </thead>
      <tr>
        <td>Version</td>
        <td>
          <xsl:value-of select="*/@itsversion"/>
        </td>
      </tr>
      <tr>
        <td>Date</td>
        <td>
          <xsl:call-template name="DateTime" />
        </td>
      </tr>
      <tr>
        <td>Current user</td>
        <td>
          <xsl:value-of select="*/@currentuser"/>
        </td>
      </tr>
      <tr>
        <td>Copyright</td>
        <td>
          <xsl:text>&#xA9;</xsl:text> Time Traveller Open Source
        </td>
      </tr>
    </table>
  </xsl:template>
</xsl:stylesheet>