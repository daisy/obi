<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:ncx="http://www.daisy.org/z3986/2005/ncx/" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <!-- Output the NCX file from the monolithic z file -->

  <xsl:output method="xml" indent="yes"
    doctype-public="-//NISO//DTD ncx 2005-1//EN"
    doctype-system="http://www.daisy.org/z3986/2005/ncx-2005-1.dtd"/>

  <xsl:template match="ncx:*">
    <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="ncx:*/text()">
    <xsl:value-of select="."/>
  </xsl:template>
  
  <xsl:template match="text()"/>

</xsl:stylesheet>