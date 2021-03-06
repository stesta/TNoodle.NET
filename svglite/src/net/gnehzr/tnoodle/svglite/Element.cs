﻿using System;
using System.Collections.Generic;
using System.Text;

using static net.gnehzr.tnoodle.svglite.Utils;

namespace net.gnehzr.tnoodle.svglite
{
    public class Element
    {
        protected String tag;
        protected LinkedHashMap<string, string> attributes;
        protected LinkedHashMap<string, string> style;
        protected List<Element> children;
        protected string content;
        public Element(String tag)
        {
            this.tag = tag;
            this.children = new List<Element>();
            this.attributes = new LinkedHashMap<string, string>();
            this.style = new LinkedHashMap<string, string>();
            this.content = null;
        }

        public Element(Element e)
        {
            this.tag = e.tag;
            this.attributes = new LinkedHashMap<string, string>(e.attributes);
            this.style = new LinkedHashMap<string, string>(e.style);
            this.children = e.copyChildren();
            //this.content = content;
        }

        protected List<Element> copyChildren()
        {
            List<Element> childrenCopy = new List<Element>();
            foreach (Element child in children)
            {
                childrenCopy.Add(new Element(child));
            }
            return childrenCopy;
        }

        public string getContent()
        {
            return content;
        }

        public void setContent(string content)
        {
            this.content = content;
        }

        public List<Element> getChildren()
        {
            return children;
        }

        public void appendChild(Element child)
        {
            children.Add(child);
        }

        public LinkedHashMap<string, string> getAttributes()
        {
            return attributes;
        }

        public string getAttribute(string key)
        {
            azzert(key != "style");
            return attributes[key];
        }

        public void setAttribute(string key, string value)
        {
            azzert(key != "style");
            attributes.Add(key, value);
        }

        public void setStyle(string key, string value)
        {
            style.Add(key, value);
        }

        public String getStyle(string key)
        {
            return style[key];
        }

        public LinkedHashMap<string, string> getStyle()
        {
            return style;
        }

        private String colorToStr(Color c)
        {
            return c == null ? "none" : "#" + c.toHex();
        }

        public String toStyleStr()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in style.Keys)
            {
                String value = style[key];
                sb.Append(" ").Append(key).Append(":").Append(value).Append(";");
            }
            if (sb.Length == 0)
            {
                return "";
            }
            return sb.ToString();
        }

        private void addIndentation(StringBuilder sb, int level)
        {
            for (int i = 0; i < level; i++)
            {
                sb.Append("\t");
            }
        }

        public virtual void buildString(StringBuilder sb, int level)
        {
            addIndentation(sb, level);
            sb.Append("<").Append(tag);
            foreach (string key in attributes.Keys)
            {
                String value = attributes[key];
                sb.Append(" ");
                sb.Append(key).Append("=").Append('"').Append(value).Append('"');
            }
            if (style.Count > 0)
            {
                sb.Append(" style=\"").Append(toStyleStr()).Append('"');
            }
            if (!_transform.isIdentity())
            {
                sb.Append(" transform=\"").Append(_transform.toSvgTransform()).Append('"');
            }
            sb.Append(">");
            if (content != null)
            {
                sb.Append(content);
            }
            foreach (Element child in children)
            {
                sb.Append("\n");
                child.buildString(sb, level + 1);
            }
            sb.Append("\n");
            addIndentation(sb, level);
            sb.Append("</").Append(tag).Append(">");
        }

        public string toString()
        {
            StringBuilder sb = new StringBuilder();
            buildString(sb, 0);
            return sb.ToString();
        }

        public void setFill(Color c)
        {
            setAttribute("fill", colorToStr(c));
        }

        public void setStroke(Color c)
        {
            setAttribute("stroke", colorToStr(c));
        }

        public void setStroke(int strokeWidth, int miterLimit, String lineJoin)
        {
            setStyle("stroke-width", strokeWidth + "px");
            setStyle("stroke-miterlimit", "" + miterLimit);
            setStyle("stroke-linejoin", lineJoin);
        }

        private Transform _transform = new Transform();
        public void transform(Transform t)
        {
            _transform.concatenate(t);
        }

        public void setTransform(Transform t)
        {
            if (t == null)
            {
                _transform.setToIdentity();
            }
            else
            {
                _transform.setTransform(t);
            }
        }

        public Transform getTransform()
        {
            return new Transform(_transform);
        }

        public void rotate(double radians, double anchorx, double anchory)
        {
            _transform.rotate(radians, anchorx, anchory);
        }

        public void rotate(double radians)
        {
            _transform.rotate(radians);
        }

        public virtual void translate(double x, double y)
        {
            _transform.translate(x, y);
        }
    }
}