<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:smil="http://www.w3.org/2001/SMIL20/" 
  xmlns:xuk="http://www.daisy.org/urakawa/xuk/1.0"
  xmlns:obi="http://www.daisy.org/urakawa/obi" 
  xmlns:ext="http://www.daisy.org/urakawa/obi/xslt-extensions" 
  exclude-result-prefixes="xuk obi"
  extension-element-prefixes="ext">
  <xsl:output method="xml"/>

  <!-- Total time of the book in milliseconds -->
  <xsl:param name="total-time"/>

  <!-- Name of the UID element -->
  <xsl:param name="uid">UID</xsl:param>


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
                  <xsl:attribute name="id">
                    <xsl:value-of select="$uid"/>
                  </xsl:attribute>
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
          <item href="{ext:RelativePath('.opf')}" media-type="text/xml"/>
          <item href="{ext:RelativePath('.ncx')}" media-type="application/x-dtbncx+xml"/>
          <xsl:for-each select="//xuk:ExternalAudioMedia">
            <xsl:variable name="src" select="@src"/>
            <xsl:if test="not(preceding::xuk:ExternalAudioMedia[@src=$src])">
              <item href="{ext:RelativePathForUri($src)}" media-type="audio/x-wav"/>
            </xsl:if>
          </xsl:for-each>
          <xsl:for-each select="//obi:section">
            <item href="{ext:RelativePath('.smil', generate-id())}" id="{generate-id()}"
              media-type="application/smil"/>
          </xsl:for-each>
        </manifest>
        <spine>
          <xsl:for-each select="//obi:section">
            <itemref idref="{generate-id()}"/>
          </xsl:for-each>
        </spine>
      </package>

      <!-- The smil files; one per section -->
      <xsl:for-each select="//obi:section">
        <smil:smil></smil:smil>
      </xsl:for-each>

    </z>
  </xsl:template>
</xsl:stylesheet>