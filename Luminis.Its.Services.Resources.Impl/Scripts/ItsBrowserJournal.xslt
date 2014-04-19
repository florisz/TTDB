<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" />
  <xsl:template match="Journal">
    <tr>
      <td>
        <xsl:value-of select="Username" />
      </td>
      <td>
        <xsl:value-of select="Timestamp" />
      </td>
      <td>
        <a>
          <xsl:attribute name="href">
            <xsl:value-of select="Link/@href" />
          </xsl:attribute>
          <xsl:value-of select="Link/@href" />
        </a>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>