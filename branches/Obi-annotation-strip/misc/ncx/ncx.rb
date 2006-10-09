#!/usr/bin/ruby

# Some quick tests in Ruby for NCX manipulation

class Content

  def initialize(src)
    @id = "content_" + self.hash.to_s
    @src = src
  end

  def to_s
    @src
  end

  def to_xml
    "<content id=\"#{@id}\" src=\"#{@src}\"/>"
  end

end

class Audio

  def initialize(src, clip_begin, clip_end)
    @id = "audio_" + self.hash.to_s
    @class = nil
    @src = src
    @clipBegin = clip_begin
    @clipEnd = clip_end
  end

  def to_s
    "@audio(#{@src}:#{@clipBegin}:#{@clipEnd})"
  end

  def to_xml
    _class = if @class.nil?
               ""
             else
               " class=\"#{@class}\""
             end
    "<audio id=\"#{@id}\"#{_class} src=\"#{@src}\" " +
      "clipBegin=\"#{@clipBegin}\" clipEnd=\"#{@clipEnd}\"/>"
  end

end

class Text

  def initialize(text)
    @id = "text_" + self.hash.to_s
    @class = nil
    @text = text
  end

  def to_s
    @text
  end

  def to_xml
    _class = if @class.nil?
               ""
             else
               " class=\"#{@class}\""
             end
    "<text id=\"#{@id}\"#{_class}>" + quote(@text) + "</text>"
  end

  def quote(str)
    str
  end

end

class NavLabel

  def initialize(text, audio)
    @text = text
    @audio = audio
    @lang = nil
    @dir = nil # :ltr or :rtl
  end

  attr_writer :text, :audio

  def NavLabel.text_only(text)
    NavLabel.new(text, nil)
  end

  def NavLabel.audio_only(audio)
    NavLabel.new(nil, audio)
  end

  def to_s
    if @text != nil 
      if @audio != nil
        "<\"#{@text}\", #{@audio}>"
      else
        "\"#{@text}\""
      end
    else
      @audio.to_s
    end
  end

  def to_xml
    "<navLabel>" + xml_text + xml_audio + "</navLabel>"
  end

  def xml_text
    if @text.nil?
      ""
    else
      @text.to_xml
    end
  end

  def xml_audio
    if @audio != nil
      @audio.to_xml
    else
      ""
    end
  end

end

class NavPoint

  def initialize(labels, content)
    @parent = nil                       # parent navpoint or navmap
    @level = 1                          # level
    @prev = nil                         # previous navpoint
    @next = nil                         # next navpoint
    @id = "navpoint_" + self.hash.to_s  # id for XML input/output
    @class = nil                        # class for XML input/output
    @labels = labels                    # navlabels
    @content = content                  # content
    @children = []                      # children navpoints
  end

  attr_reader :parent, :level, :prev, :next
  attr_writer :parent, :level, :prev, :next

  # Append a child to the list of children and maintain the links between
  # navpoints if the flag is set to true.
  def append_child(child, maintain_params = true)
    child.parent = self
    if maintain_params
      child.level = @level + 1
      last = last_descendant_or_self
      child.prev = last
      last.next, child.next = child, last.next
      child.next.prev = child if child.next != nil
    end
    @children.push(child)
  end

  # Show this navpoint and the following ones. Call this method on the
  # navmap to show the full navigation map
  def show(order = 0)
    puts "#{order}\t" + ("  " * @level) +
      "#{self} <- #{@prev} -> #{@next} < #{@parent}"
    @next.show(order + 1) unless @next.nil?
  end

  # XML output
  def to_xml(order)
    _class = ""
    _class = " class=\"#{@class}\"" if @class
    str = "<navPoint id=\"#{@id}\"#{_class} playOrder=\"#{order}\">" +
      @labels.map{ |label| label.to_xml }.join("")
    str += @content.to_xml
    @children.each { |child| order, s = child.to_xml(order + 1); str += s }
    [order, str + "</navPoint>"]
  end

  # Stringify the navigation point
  def to_s
    "-#{@level}- " + @labels.join("/") # + " = #{@content}"
  end

  # Attempt to move a navigation point up in the navmap. Return true if the
  # move was succesfull; otherwise, return false. Nothing has changed in
  # case of failure.
  # The move can happen iff a) the navpoint is not the first, b) the
  # navpoint before the previous (for this purpose the navmap comes before
  # the first node) has a level that is at least self's level minus 1 (again
  # the navmap's level is 0 for this purpose; the first navpoint's is 1) and
  # c) the previous navpoint's level is at most self's level plus 1.
  def move_up
    if @prev.prev and @prev.prev.level >= @level - 1 and
      @prev.level <= @level + 1
      parent = @prev.prev
      sibling = nil
      while parent.level != @level - 1
        sibling, parent = parent, parent.parent
      end
      @parent.remove_child(self)
      parent.add_child_after(self, sibling)
      # Altering the links is a bit tedious... oh well.
      _next = @next
      prev = @prev
      @prev.prev.next = self
      @next = @prev
      @prev.next = _next
      _next.prev = @prev if _next
      @prev = @prev.prev
      prev.prev = self
      true
    else
      false
    end
  end

  # Moving down is implemented by moving up the next element, if it exists.
  def move_down
    if @next
      @next.move_up
    else
      false
    end
  end

  # Decrease the level of a navpoint. This only works if it has no children
  # and the level is higher than 1 (i.e. there is a grand parent)
  def decrease_level
    if @parent.parent and @children.empty?
      siblings = @parent.remove_child_and_siblings(self)
      parent = @parent
      @parent = parent.parent
      @parent.add_child_after(self, parent)
      siblings.each { |child| self.append_child(child, false) }
      @level -= 1
      true
    else
      false
    end
  end

  # Increase the level: only possible if the node has a previous sibling.
  # Then that sibling becomes the parent of the node and of its children.
  # Return true if the change could be made, false otherwise.
  def increase_level
    sibling = @parent.previous_sibling(self)
    if sibling != nil
      @parent.remove_child(self)
      @parent = sibling
      @parent.append_child(self, false)
      @children.each { |child| @parent.append_child(child, false) }
      @children.clear
      @level += 1
      true
    else
      false
    end
  end

  # Find the last descendant of this navpoint, or itself if it has no
  # children
  def last_descendant_or_self
    if @children.empty?
      self
    else
      @children[-1].last_descendant_or_self
    end
  end

  # Remove a child node from the list of children. The parent/prev/next links
  # are not updated; this is the responsability of the caller.
  def remove_child(child)
    @children.delete(child)
  end

  # Remove a child node and all its following siblings and return the list
  # of siblings
  def remove_child_and_siblings(child)
    i = @children.index(child)
    siblings = @children.slice!(i + 1 .. -1)
    @children.delete_at(i)
    siblings
  end

  # Add a child node after a given child node; if nil, add at the beginning
  # of the list of children. The parent/prev/next links are not updated.
  def add_child_after(new, existing)
    if existing
      @children[@children.index(existing) + 1, 0] = new
    else
      @children.unshift(new)
    end
  end

  # Find the previous sibling of a child. Return nil if it is the first
  # child.
  def previous_sibling(child)
    index = @children.index(child)
    if index and index > 0
      @children[index - 1]
    else
      nil
    end
  end

  # Delete a navpoint. Only a navpoint with no children can be removed.
  # Return true if the change could be made, false otherwise.
  def remove
    if @children.empty?
      @parent.remove_child(self)
      @prev.next = @next if @prev
      @next.prev = @prev if @next
      true
    else
      false
    end
  end

end

class NavMap < NavPoint

  def initialize
    @parent = nil
    @prev = nil
    @next = nil
    @level = 0
    @id = "navmap_" + self.hash.to_s
    @children = []
  end

  def to_s
    "-0- navMap"
  end

  # Produce an XML fragment for the navigation map. InfoLabel is missing at
  # the moment.
  def to_xml
    order = 0
    str = "<navMap id=\"#{@id}\">"
    @children.each { |child| order, s = child.to_xml(order + 1); str += s }
    str + "</navMap>"
  end
  
end

class NCX

  def initialize
    @navmap = NavMap.new
    navpoint = NavPoint.new([NavLabel.text_only(Text.new("Title"))])
    @navmap.append_child(navpoint)
  end

  attr_reader :navmap

end

ncx = NCX.new
#ncx.navmap.show


#navs = ["A", "B", "C", "D", "E", "F", "G"].map {
  #  |l| NavPoint.new([NavLabel.text_only(Text.new(l))], 
  #                 Content.new("foo.smil#" + l))
  #}
  #navmap = NavMap.new
  #navmap.append_child(navs[0])
  #navmap.append_child(navs[5])
  #navs[0].append_child(navs[1])
  #navs[1].append_child(navs[2])
  #navs[0].append_child(navs[4])
  #navs[1].append_child(navs[3])
  #navs[5].append_child(navs[6])
  #navs[5].increase_level
  #navs[5].increase_level
  #navs[5].increase_level
  #navmap.show
#puts navmap.to_xml
