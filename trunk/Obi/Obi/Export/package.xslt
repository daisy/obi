<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:opf="http://openebook.org/namespaces/oeb-package/1.0/">

<!-- Output the package file from the monolithic z file -->

  <xsl:output method="xml"
    doctype-public="+//ISBN 0-9673008-1-9//DTD OEB 1.2 Package//EN"
    doctype-system="oebpkg12.dtd"/>

  <xsl:template match="opf:*">
    <xsl:copy>
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>