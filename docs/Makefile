# Make markdown documentation
# $Id$

XSLT =		xsltproc
STYLESHEET =	template.xslt
HTML_FILES =	events.html index.html strip_manager_shortcuts.html

all:	$(HTML_FILES)

%.html:	%.txt
	echo "<markdown>" > $@.markdown && \
	markdown $< >> $@.markdown && \
	echo "</markdown>" >> $@.markdown && \
	$(XSLT) -o $@ $(STYLESHEET) $@.markdown && \
	$(RM) $@.markdown

.PHONY:	clean

clean:
	$(RM) *.markdown $(HTML_FILES)
