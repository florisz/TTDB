<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" />

  <xsl:include href="../scripts/itsbrowserhead.xslt" />
  <xsl:include href="../scripts/itsbrowserpageheader.xslt" />
  <xsl:include href="../scripts/itsbrowserlink.xslt" />

  <xsl:template match="/">
    <html>
      <xsl:call-template name="Head" />
      <body>
        <xsl:call-template name="PageHeader" />
        <h3>
          <xsl:value-of select="name(*[1])" />
        </h3>
        <table>
          <thead>
            <td>Name</td>
            <td>Link</td>
          </thead>
          <xsl:apply-templates select="*/Link" />
        </table>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>