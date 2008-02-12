<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xuk="http://www.daisy.org/urakawa/xuk/1.0"
  exclude-result-prefixes="xuk">
  <xsl:output method="xml"/>
  <xsl:param name="total-time"/>
  <xsl:param name="uid"/>
  <xsl:template match="*">
    <!-- wrapper for the whole fileset -->
    <z>
      <!-- the package file -->
      <package xmlns="http://openebook.org/namespaces/oeb-package/1.0/"
        unique-identifier="{$uid}">
        <metadata>
          <dc-metadata xmlns:dc="http://purl.org/dc/elements/1.1/"
            xmlns:oebpackage="http://openebook.org/namespaces/oeb-package/1.0/">
            <xsl:for-each select="//xuk:Metadata[starts-with(@name,'dc:')]">
              <xsl:element name="{@name}">
                <xsl:if test="@name='dc:Identifier'">
                  <xsl:attribute name="id"><xsl:value-of select="$uid"/></xsl:attribute>
                  <xsl:attribute name="scheme">DTB</xsl:attribute>
                </xsl:if>
                <xsl:value-of select="@content"/>
              </xsl:element>
            </xsl:for-each>
            <dc:Format>ANSI/NISO Z39.86-2005</dc:Format>
            <dc:Type>Sound</dc:Type>
          </dc-metadata>
          <x-metadata>
            <xsl:for-each select="//xuk:Metadata[not(starts-with(@name,'dc:'))]">
              <meta name="{@name}" content="{@content}"/>
            </xsl:for-each>
            <meta name="dtb:multimediaType" content="audioNCX"/>
            <meta name="dtb:multimediaContent" content="audio"/>
            <meta name="dtb:totalTime" content="{$total-time}ms"/>
            <meta name="dtb:audioFormat" content="WAV"/>
          </x-metadata>
        </metadata>
        <manifest>
          
        </manifest>
        <spine>
          
        </spine>
      </package>
    </z>
  </xsl:template>
</xsl:stylesheet>