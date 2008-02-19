<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:oebpackage="http://openebook.org/namespaces/oeb-package/1.0/"
  xmlns:dc="http://purl.org/dc/elements/1.1/">

  <!-- Output the package file from the monolithic z file -->

  <xsl:output method="xml" indent="yes"
    doctype-public="+//ISBN 0-9673008-1-9//DTD OEB 1.2 Package//EN"
    doctype-system="http://openebook.org/dtds/oeb-1.2/oebpkg12.dtd"/>

  <xsl:template match="oebpackage:dc-metadata">
    <dc-metadata xmlns="http://openebook.org/namespaces/oeb-package/1.0/"  xmlns:dc="http://purl.org/dc/elements/1.1/">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </dc-metadata>
  </xsl:template>

  <xsl:template match="oebpackage:*">
    <xsl:element name="{local-name()}" namespace="{namespace-uri()}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="dc:*">
    <xsl:element name="dc:{local-name()}">
      <xsl:copy-of select="@*"/>
      <xsl:value-of select="."/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="text()"/>

</xsl:stylesheet>