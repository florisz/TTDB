<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:include href="../scripts/itsbrowserhead.xslt" />
  <xsl:include href="../scripts/itsbrowserpageheader.xslt" />
  <xsl:include href="../scripts/itsbrowserjournal.xslt" />

  <xsl:template match="/">
    <html>
      <xsl:call-template name="Head" />
      <body>
        <xsl:call-template name="PageHeader" />
        <table>
          <thead>
            <td>Who</td>
            <td>When</td>
            <td>Link</td>
          </thead>
          <xsl:apply-templates select="*/Journal" />
        </table>
      </body>
    </html>
  </xsl:template>
  
</xsl:stylesheet>
