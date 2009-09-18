<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:smil="http://www.w3.org/2001/SMIL20/">

  <!-- Output a SMIL file from the monolithic z file -->

  <!-- ID of the SMIL element to match (one smil element per file) -->
  <xsl:param name="id"/>

  <xsl:output method="xml" indent="yes"
    doctype-public="-//NISO//DTD dtbsmil 2005-1//EN"
    doctype-system="http://www.daisy.org/z3986/2005/dtbsmil-2005-1.dtd"/>

  <xsl:template match="smil:smil">
    <xsl:if test="smil:body/smil:seq[@id=$id]">
      <xsl:apply-templates mode="copy" select="."/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="text()"/>

  <xsl:template match="smil:*" mode="copy">
    <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates mode="copy"/>
    </xsl:element>
  </xsl:template>

</xsl:stylesheet>