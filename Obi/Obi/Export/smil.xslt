<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:smil="http://www.w3.org/2001/SMIL20/">

  <!-- Output a SMIL file from the monolithic z file -->

  <xsl:output method="xml"
    doctype-public="-//NISO//DTD dtbsmil 2005-1//EN"
    doctype-system="http://www.daisy.org/z3986/2005/dtbsmil-2005-1.dtd"/>

  <xsl:template match="smil:*">
    <xsl:copy>
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>