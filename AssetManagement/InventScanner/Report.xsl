<?xml version="1.0" encoding="ISO-8859-1"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
  <html>
  <body>
  <h2>Synchronization report</h2>
  <table border="1">
    <tr bgcolor="#9acd32">
      <th>Name</th>
      <th>Barcode</th>
      <th>Message</th>
    </tr>
    <xsl:for-each select="ArrayOfReportRow/ReportRow">
    <tr>
      <td><xsl:value-of select="Name"/></td>
      <td><xsl:value-of select="Barcode"/></td>
      <td><xsl:value-of select="SyncMessage"/></td>
    </tr>
    </xsl:for-each>
  </table>
  </body>
  </html>
</xsl:template>

</xsl:stylesheet>